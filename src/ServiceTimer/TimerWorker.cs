
#if DEBUG
#define BASELOG
#endif

namespace ServiceTimer
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Common.Extensions;
    using NLog;
    using SharpRaven;
    using SharpRaven.Data;
    using Simple.OData.Client;

    /// <summary>
    ///     Inherit from this class to create your concrete worker class. IDisposable because
    ///     System.Timers.Timer, used within, is IDisposable
    /// </summary>
    public abstract class TimerWorker : IDisposable
    {
        /// <summary>
        ///     Storing a count of the number of times the timer has elapsed
        /// </summary>
        private uint _elapseCount;

        /// <summary>
        ///     The nlog logger
        /// </summary>
        private Logger _logger;

        /// <summary>
        ///     If true, the worker has "paused" (as a result of the Service being Paused)
        ///     While this is true, the timer continues to elapse, but the Work method
        ///     shall not be called.
        ///     If false, the worker is running normally
        /// </summary>
        private bool _paused;

        /// <summary>
        ///     Track whether or not one call has been made to StartWork()
        /// </summary>
        private bool _startedWork;

        /// <summary>
        ///     The timer
        /// </summary>
        private System.Timers.Timer _timer;

        /// <summary>
        ///     The timer interval
        /// </summary>
        private double _timerInterval;

        /// <summary>
        ///     Track the total number of times the Timer has elapsed. For information purposes
        /// </summary>
        private ulong _totalElapseCount;

        /// <summary>
        ///     Defines when Work shall be called to faciliate carrying
        ///     out work. For example, if "_workOnElapseCount == 5" the Work method
        ///     will raised on every 5th elapse of the timer; and every other time
        ///     the timer elapses, a check will be carried out to see if the
        ///     service is Stopping, but Work() will not be called.
        /// </summary>
        private uint _workOnElapseCount;

        protected RavenClient RavenClient;

        private TimerWorker()
        {
            // Hide the default constructor
        }

        /// <summary>
        ///     Constructor
        ///     timerInterval: the timer interval. When the time elapses, other than on a workOnElapseCount,
        ///     the service state is evaluated.
        ///     workOnElapseCount: the number of times the timer must elapse before the event is raised
        ///     to cause work to be carried out
        /// </summary>
        /// <param name="timerInterval"></param>
        /// <param name="workOnElapseCount"></param>
        protected TimerWorker(double timerInterval, uint workOnElapseCount)
        {
            RavenClient = Core.Sentry.RavenClient.New();
            _TimerWorker(0, timerInterval, workOnElapseCount);
        }

        /// <summary>
        ///     As above, but with a delayed start
        /// </summary>
        /// <param name="delayOnStart"></param>
        /// <param name="timerInterval"></param>
        /// <param name="workOnElapseCount"></param>
        protected TimerWorker(double delayOnStart, double timerInterval, uint workOnElapseCount)
        {
            RavenClient = Core.Sentry.RavenClient.New();
            _TimerWorker(delayOnStart, timerInterval, workOnElapseCount);
        }

        public string FailedMessage { get; set; }

        /// <summary>
        ///     Delegate call back to the service to check on its state
        /// </summary>
        internal TimerServiceBase.GetServiceStateDelegate GetServiceStateHandler { get; set; }

        /// <summary>
        ///     Log
        /// </summary>
        protected Logger Logger => _logger;

        /// <summary>
        ///     Check if the state of the service is something other than "Running"
        /// </summary>
        protected bool ServiceStateRequiresStop => GetServiceStateHandler() != TimerServiceBase.ServiceState.Running;

        /// <summary>
        ///     The signal used by this worker when the Service is waiting for the worker
        ///     to handle a change in state
        /// </summary>
        internal ManualResetEvent SignalEvent { get; set; }

        public string SuccessMessage { get; set; }

        private void _doWork(TimerWorkerInfo info)
        {
#if BASELOG
            DateTime entered = DateTime.UtcNow;

            _logger?.Info(logmessages.WorkerWorkEnter,
                info.WorkId,
                info.ThreadId,
                entered.ToString("u"));
#endif

            try
            {
                if (!_startedWork)
                {
                    StartWork(info);
                }

                try
                {
                    Work(info);
                }
                catch (TaskCanceledException ex)
                {
                    if (ex.CancellationToken.IsCancellationRequested)
                    {
                        Logger.Error(ex.Message);
                    }
                    else
                    {
                        Logger.Error("Http request timeout.");
                    }
                }
                catch (WebRequestException ex)
                {
                    ex.Handle(Logger);
                }
                catch (IOException ex)
                {
                    // chances are the reason this is thrown is because the client is low on disk space.
                    // therefore we output to console instead of the logger.

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(FailedMessage);
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    RavenClient.Capture(new SentryEvent(ex));
                    Logger.Error(ex.Message);
                    Logger.Error(FailedMessage);
                }
            }
            catch (Exception ex)
            {
                var wex = new OnWorkException(logmessages.WorkerOnWorkException,
                    ex, info);

                _logger?.Warn(logmessages.WorkerOnWorkException, wex);

                throw wex;
            }
            finally
            {
                _startedWork = true;
            }

#if BASELOG
            if (_logger != null)
            {
                DateTime exited = DateTime.UtcNow;

                _logger.Info(logmessages.WorkerWorkExit,
                    info.WorkId,
                    info.ThreadId,
                    exited.ToString("u"));

                TimeSpan duration =
                    exited - entered;

                _logger.Info(logmessages.WorkerWorkDuration,
                    info.WorkId,
                    info.ThreadId,
                    duration);
            }
#endif
        }

        /// <summary>
        ///     This function handles incrementing and resetting the elapse count.
        /// </summary>
        /// <param name="doWork">If true, the Work method will be called. If false, it won't.</param>
        private TimerWorkerInfo _ElapseIncrement(out bool doWork)
        {
            doWork = false;

            _elapseCount++;

            _totalElapseCount++;

            // Create a TimerWorkerInfo class for information purposes
            TimerWorkerInfo info = TimerWorkerInfo.Info
                (_timerInterval, _elapseCount, _workOnElapseCount, _totalElapseCount);

#if BASELOG
            _logger?.Info($"TimerWorkerInfo: {info}");
#endif

            // Reset the counter if necessary
            if (_elapseCount >= _workOnElapseCount)
            {
                _elapseCount = 0;

                doWork = true;
            }

            return info;
        }

        private void _QueryAndHandleServiceState(TimerWorkerInfo info, out bool doWork, out bool stop)
        {
            // Query the state of the service

            TimerServiceBase.ServiceState state
                = GetServiceStateHandler();

            // Handle the state appropriately...

            if (state == TimerServiceBase.ServiceState.Running)
            {
                // The service is running normally. If this worker is paused, 
                //  call the OnContinue method. The use of a 
                //  try {} .. finally {} here ensures that the transition
                //  occurs, regardless of what happens in OnContinue()
                try
                {
                    if (_paused)
                    {
                        _paused = false;

                        OnContinue(info);
                    }
                }
                catch (Exception ex)
                {
                    var cex = new OnContinueException(logmessages.WorkerOnContinueException,
                        ex, info);

                    if (_logger != null)
                    {
                        _logger.Warn(logmessages.WorkerOnContinueException, cex);
                    }

                    throw cex;
                }
                finally
                {
                    doWork = true;

                    stop = false;
                }
            }

            else if (state == TimerServiceBase.ServiceState.Paused)
            {
                // The service is paused

                doWork = false;

                stop = false;
            }

            else if (state == TimerServiceBase.ServiceState.Pausing)
            {
                // The service is pausing. If this worker is already 
                //  paused, nothing else need be done

                if (_paused)
                {
                    doWork = false;

                    stop = false;
                }
                else
                {
                    // The service is pausing but this worker is not paused
                    // Call the pause method, pause the worker, and set the 
                    //  signal to tell the Service that this worker has 
                    //  transitioned to a paused state. The use of a 
                    //  try {} .. finally {} here ensures that the transition
                    //  occurs, regardless of what happens in OnPause()

                    try
                    {
                        OnPause(info);
                    }
                    catch (Exception ex)
                    {
                        var pex = new OnPauseException(logmessages.WorkerOnPauseException,
                            ex, info);

                        if (_logger != null)
                        {
                            _logger.Warn(logmessages.WorkerOnPauseException, pex);
                        }

                        throw pex;
                    }
                    finally
                    {
                        _paused = true;

                        SignalEvent.Set();

                        doWork = false;

                        stop = false;
                    }
                }
            }

            else if (state == TimerServiceBase.ServiceState.Stopping)
            {
                // The service is stopping. Call the stop method, 
                //  and set the signal to tell the Service that this worker has 
                //  transitioned to a stopped state. The use of a 
                //  try {} .. finally {} here ensures that the transition
                //  occurs, regardless of what happens in OnStop()

                try
                {
                    OnStop(info);
                }
                catch (Exception ex)
                {
                    var sex = new OnStopException(logmessages.WorkerOnStopException,
                        ex, info);

                    if (_logger != null)
                    {
                        _logger.Warn(logmessages.WorkerOnStopException, sex);
                    }

                    throw sex;
                }
                finally
                {
                    SignalEvent.Set();

                    doWork = false;

                    stop = true;
                }
            }

            else if (state == TimerServiceBase.ServiceState.ShuttingDown)
            {
                // The service is stopping due to a shut down. Call the Shutdown method, 
                //  and set the signal to tell the Service that this worker has 
                //  transitioned to a stopped state. The use of a 
                //  try {} .. finally {} here ensures that the transition
                //  occurs, regardless of what happens in OnShutDown()

                try
                {
                    OnShutdown(info);
                }
                catch (Exception ex)
                {
                    var sex = new OnShutdownException(logmessages.WorkerOnShutdownException,
                        ex, info);

                    if (_logger != null)
                    {
                        _logger.Warn(logmessages.WorkerOnShutdownException, sex);
                    }

                    throw sex;
                }
                finally
                {
                    SignalEvent.Set();

                    doWork = false;

                    stop = true;
                }
            }

            else
            {
                throw new NotImplementedException("It should be impossible for execution to reach here");
            }
        }

        /// <summary>
        ///     Fires every time that the timer elapses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool stopWorker = false;

            try
            {
                // Stop the timer, so that it doesn't fire again while the code 
                //  in this event (including any work to be done) is executing

                ((System.Timers.Timer) sender).Stop();

                // Ensure the timer interval is correctly set. We may have 
                //  incorporated a Delay at class construction

                _timer.Interval = _timerInterval;

                // Handle the elapsed counter and assess if it is time to call 
                // the Work method
                TimerWorkerInfo info = _ElapseIncrement(out bool workOnElapse);

                // Query the service state and handle it as necessary
                _QueryAndHandleServiceState(info, out bool workOnState, out stopWorker);

                // Call the work method to do any work, if now is the right time
                if (!stopWorker
                    && workOnElapse
                    && workOnState)
                {
                    _doWork(info);
                }
            }

            // If for some reason you want exceptions to be thrown up the stack from here, which is likely to 
            //   be fatal to your service, remove the NOFATALITY conditional compilation argument
            //      in Project properties. Otherwise, exceptions are logged &/or suppressed
#if !NOFATALITY
            catch
            {
                throw;
#else
            catch (Exception ex)
            {
                _logger?.Error(ex, logmessages.WorkerException);
#endif
            }
            finally
            {
                // Start the timer again, if it's appropriate to do so
                if (!stopWorker)
                {
                    ((System.Timers.Timer) sender).Start();
                }
            }
        }

        /// <summary>
        ///     Runs on construction
        /// </summary>
        /// <param name="delayOnStart"></param>
        /// <param name="timerInterval"></param>
        /// <param name="workOnElapseCount"></param>
        private void _TimerWorker(double delayOnStart, double timerInterval, uint workOnElapseCount)
        {
            // With a value of 0, no work would ever be done
            if (workOnElapseCount < 1)
                throw new ArgumentOutOfRangeException(nameof(workOnElapseCount));

            // The timer interval is initially set to timerInterval + delayOnStart, so that any 
            //  delay is initially observed; after its first elapse the timer will fall back 
            //  to the timerInterval value alone.

            _timer = new System.Timers.Timer();

            _timer.Interval = timerInterval + delayOnStart;

            _timer.Elapsed += _timerElapsed;

            _timerInterval = timerInterval;

            _workOnElapseCount = workOnElapseCount;

            _paused = false;

            _startedWork = false;
        }

        protected virtual void OnContinue(TimerWorkerInfo info)
        {
#if BASELOG
            _logger?.Info(logmessages.WorkerOnContinue,
                info.WorkId,
                info.ThreadId);
#endif
        }

        protected virtual void OnPause(TimerWorkerInfo info)
        {
#if BASELOG
            _logger?.Info(logmessages.WorkerOnPause,
                info.WorkId,
                info.ThreadId);
#endif
        }

        protected virtual void OnShutdown(TimerWorkerInfo info)
        {
#if BASELOG
            _logger?.Info(logmessages.WorkerOnShutdown,
                info.WorkId,
                info.ThreadId);
#endif
        }

        protected virtual void OnStop(TimerWorkerInfo info)
        {
#if BASELOG
            _logger?.Info(logmessages.WorkerOnStop,
                info.WorkId,
                info.ThreadId);
#endif
        }

        /// <summary>
        ///     Get/Set the log4net logger
        /// </summary>
        /// <param name="log"></param>
        internal void SetLog(Logger log)
        {
            _logger = log;
        }

        /// <summary>
        ///     Start the timer
        /// </summary>
        internal void StartTimer()
        {
            _timer.Start();
        }

        protected abstract void StartWork(TimerWorkerInfo info);

        protected abstract void Work(TimerWorkerInfo info);

        #region "Dispose"

        // Always implement Dispose correctly and in accordance with best practise

        private bool _disposed;

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _timer?.Dispose();
                // Free any other managed objects here. 
                //
#if DEBUG
                // Evidence that this gets disposed of in the logs
                Logger?.Info($"Base disposed of managed resources occurred in type {GetType().Name}");
#endif
            }
            // Free any unmanaged objects here. 
            //
            _disposed = true;
        }

        #endregion
    }
}