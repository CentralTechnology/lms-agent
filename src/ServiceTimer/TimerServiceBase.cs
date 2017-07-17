
#if DEBUG

#define BASELOG

#endif

namespace ServiceTimer
{
    using System;
    using System.Collections;
    using System.Threading;
    using NLog;

    /// <summary>
    ///     A class that derives from ServiceBase, and that you can derive from, for developing NT-style services
    ///     that use Timer-based workers. Theoretically, this would be an abstract class,
    ///     but making it so "breaks" the VS designer component for Services.
    /// </summary>
    public class TimerServiceBase
    {
        /// <summary>
        ///     A reference object to facilitate thread-locking the _serviceState
        ///     enum var declared immediately above
        /// </summary>
        private readonly object _serviceStateLock = new object();

        /// <summary>
        ///     The log4net logger
        /// </summary>
        private Logger _log;

        /// <summary>
        ///     Track the state of this service
        /// </summary>
        private ServiceState _serviceState = ServiceState.Running;

        /// <summary>
        ///     Collection of worker classes, all of which inherit from the
        ///     abstract TimerWorker
        /// </summary>
        private ArrayList _workers;

        /// <summary>
        ///     Get all of the signals used for transitioning between states
        /// </summary>
        private WaitHandle[] _getSignals
        {
            get
            {
                var handleArray = new ArrayList();

                if (_workers != null && _workers.Count > 0)
                {
                    foreach (WorkerCollectionItem i in _workers)
                    {
                        if (i.Worker?.signalEvent != null)
                        {
                            handleArray.Add(i.Worker.signalEvent);
                        }
                    }
                }

                return (ManualResetEvent[]) handleArray.ToArray(typeof(ManualResetEvent));
            }
        }

        /// <summary>
        ///     Log
        /// </summary>
        protected Logger Log => _log;

        /// <summary>
        ///     Register a worker
        /// </summary>
        /// <param name="worker"></param>
        private void _registerWorker(TimerWorker worker)
        {
            // Provide the worker with a handler to the function that 
            //  allows it to evaluate the state of this service

            worker.getServiceStateHandler = getServiceState;

            // If this service is using a logger, set a logger for the worker too
            if (_log != null)
            {
                worker.SetLog(LogManager.GetLogger(worker.GetType().Name));
            }

            // Add it to the collection 

            if (_workers == null)
                _workers = new ArrayList();

            _workers.Add(new WorkerCollectionItem(worker)); // The use of this constructor will cause an 
            //  associated signal (ManualResetEvent) to be created
            //  See WorkerCollectionItem.cs
            // Start the worker timer
            worker.StartTimer();
        }

        /// <summary>
        ///     Reset the signals used for transitioning between states
        /// </summary>
        private void _resetSignals()
        {
            foreach (WaitHandle waitHandle in _getSignals)
            {
                var h = (ManualResetEvent) waitHandle;
                h.Reset();
            }

#if BASELOG
            _log?.Info(logmessages.ServiceSignalsReset);
#endif
        }

        /// <summary>
        ///     Wait for all of the signals to be set
        /// </summary>
        private void _waitSignals()
        {
#if BASELOG
            _log?.Info(logmessages.ServiceSignalsWait);
#endif

            WaitHandle.WaitAll(_getSignals);

#if BASELOG
            _log?.Info(logmessages.ServiceSignalsSet);
#endif
        }

        public void Continue()
        {
            // Set the service state to "Running". Each worker will detect the change
            //  and respond accordingly. Reset signals, also.
            _log?.Info(logmessages.ServiceOnContinue);

            _resetSignals();

            lock (_serviceStateLock)
            {
                _serviceState = ServiceState.Running;
            }
        }

        protected void DefaultLog()
        {
            LogManager.ReconfigExistingLoggers();

            _log = LogManager.GetLogger(GetType().Name);
        }

        /// <summary>
        ///     Get the state of this service. Signature matches
        ///     with the delegate declared immediately above
        /// </summary>
        /// <returns></returns>
        internal ServiceState getServiceState()
        {
            lock (_serviceStateLock)
            {
                return _serviceState;
            }
        }

        public void Pause()
        {
            _log?.Info(logmessages.ServiceOnPause);

            // Reset the signals for state transitions
            _resetSignals();

            // Change the state of the service to "Pausing"
            lock (_serviceStateLock)
            {
                _serviceState = ServiceState.Pausing;
            }

            // Wait for all of the workers to pause. This means using a WaitAll()
            //  that runs across all of the signals in the Workers collection

            _waitSignals();

            // Change the state of the service to "Paused"
            lock (_serviceStateLock)
            {
                _serviceState = ServiceState.Paused;
            }

            _log?.Info(logmessages.ServicePaused);
        }

        protected void RegisterWorker(TimerWorker worker)
        {
            _registerWorker(worker);
        }

        /// <summary>
        ///     Set the log4net logger
        /// </summary>
        /// <param name="log"></param>
        protected void SetLog(Logger log)
        {
            LogManager.ReconfigExistingLoggers();

            _log = log;
        }

        public void Shutdown()
        {
            _log?.Info(logmessages.ServiceOnShutdown);

            // Reset the signals for state transitions
            _resetSignals();

            // Change the state of the service to "ShuttingDown"
            lock (_serviceStateLock)
            {
                _serviceState = ServiceState.ShuttingDown;
            }

            // Wait for all of the workers to stop. This means using a WaitAll()
            //  that runs across all of the signals in the Workers collection

            _waitSignals();

            // Change the state of the service to "Stopped"
            lock (_serviceStateLock)
            {
                _serviceState = ServiceState.Stopped;
            }

            _log?.Info(logmessages.ServiceStopped);
        }

        public TimerServiceBase()
        {
            DefaultLog();

            _log?.Info(logmessages.ServiceInitialised);
        }

        /// <inheritdoc />
        public virtual bool Start()
        {
            throw new NotImplementedException();
        }

        public bool Stop()
        {
            _log?.Info(logmessages.ServiceOnStop);

            // Reset the signals for state transitions
            _resetSignals();

            // Change the state of the service to "Stopping"
            lock (_serviceStateLock)
            {
                _serviceState = ServiceState.Stopping;
            }

            // Wait for all of the workers to stop. This means using a WaitAll()
            //  that runs across all of the signals in the Workers collection

            _waitSignals();

            // Change the state of the service to "Stopped"
            lock (_serviceStateLock)
            {
                _serviceState = ServiceState.Stopped;
            }

            _log?.Info(logmessages.ServiceStopped);

            return true;
        }

        /// <summary>
        ///     Enum to describe the states of the Service
        /// </summary>
        internal enum ServiceState
        {
            Running = 0,
            Pausing = 1,
            Paused = 2,
            Stopping = 3,
            ShuttingDown = 4,
            Stopped = 5
        }

        /// <summary>
        ///     Delegate definition for the function call made by workers
        ///     to get the state of the service
        /// </summary>
        /// <returns></returns>
        internal delegate ServiceState getServiceStateDelegate();
    }
}