namespace ServiceTimer
{
    using System;

    public class TimerWorkException : Exception
    {
        public TimerWorkException()
        {
        }

        public TimerWorkException(string message)
            : base(message)
        {
        }

        public TimerWorkException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public TimerWorkException(string message, Exception inner, TimerWorkerInfo info)
            : base(message, inner)
        {
            TimerWorkerInfo = info;
        }

        public TimerWorkerInfo TimerWorkerInfo { private set; get; }
    }

    public class OnPauseException : TimerWorkException
    {
        public OnPauseException()
        {
        }

        public OnPauseException(string message)
            : base(message)
        {
        }

        public OnPauseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public OnPauseException(string message, Exception inner, TimerWorkerInfo info)
            : base(message, inner, info)
        {
        }
    }

    public class OnContinueException : TimerWorkException
    {
        public OnContinueException()
        {
        }

        public OnContinueException(string message)
            : base(message)
        {
        }

        public OnContinueException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public OnContinueException(string message, Exception inner, TimerWorkerInfo info)
            : base(message, inner, info)
        {
        }
    }

    public class OnStopException : TimerWorkException
    {
        public OnStopException()
        {
        }

        public OnStopException(string message)
            : base(message)
        {
        }

        public OnStopException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public OnStopException(string message, Exception inner, TimerWorkerInfo info)
            : base(message, inner, info)
        {
        }
    }

    public class OnShutdownException : TimerWorkException
    {
        public OnShutdownException()
        {
        }

        public OnShutdownException(string message)
            : base(message)
        {
        }

        public OnShutdownException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public OnShutdownException(string message, Exception inner, TimerWorkerInfo info)
            : base(message, inner, info)
        {
        }
    }

    public class OnWorkException : TimerWorkException
    {
        public OnWorkException()
        {
        }

        public OnWorkException(string message)
            : base(message)
        {
        }

        public OnWorkException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public OnWorkException(string message, Exception inner, TimerWorkerInfo info)
            : base(message, inner, info)
        {
        }
    }
}