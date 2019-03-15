using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public class ThreadSafeRandom
    {
        private readonly Random _random = new Random();
        private SpinLock _spinLock = new SpinLock();

        public int Next()
        {
            bool lockTaken = false;
            this._spinLock.Enter(ref lockTaken);
            try
            {
                return this._random.Next();
            }
            finally
            {
                if (lockTaken)
                    this._spinLock.Exit();
            }
        }

        public int Next(int max)
        {
            bool lockTaken = false;
            this._spinLock.Enter(ref lockTaken);
            try
            {
                return this._random.Next(max);
            }
            finally
            {
                if (lockTaken)
                    this._spinLock.Exit();
            }
        }

        public int Next(int min, int max)
        {
            bool lockTaken = false;
            this._spinLock.Enter(ref lockTaken);
            try
            {
                return this._random.Next(min, max);
            }
            finally
            {
                if (lockTaken)
                    this._spinLock.Exit();
            }
        }

        public double NextDouble()
        {
            bool lockTaken = false;
            this._spinLock.Enter(ref lockTaken);
            try
            {
                return this._random.NextDouble();
            }
            finally
            {
                if (lockTaken)
                    this._spinLock.Exit();
            }
        }

        public void NextBytes(byte[] buffer)
        {
            bool lockTaken = false;
            this._spinLock.Enter(ref lockTaken);
            try
            {
                this._random.NextBytes(buffer);
            }
            finally
            {
                if (lockTaken)
                    this._spinLock.Exit();
            }
        }
    }
}
