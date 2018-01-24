namespace LMS.Common.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abp.Dependency;
    using Abp.Domain.Services;

    public class OperationManager : DomainService, ISingletonDependency
    {
        private const int OffSet = 5;
        private readonly object _listLock = new object();
        protected internal List<int> MinutesHistory { get; private set; }

        /// <summary>
        ///     Add's the given <paramref name="minute" /> value to the internal list.
        /// </summary>
        /// <param name="minute"></param>
        public void Add(int minute)
        {
            lock (_listLock)
            {
                if (MinutesHistory == null)
                {
                    MinutesHistory = new List<int>();
                }

                if (!MinutesHistory.Any())
                {
                    MinutesHistory.Add(minute);
                    return;
                }

                if (MinutesHistory.Count >= 5)
                {
                    MinutesHistory.RemoveAt(0);
                }

                MinutesHistory.Add(minute);
            }
        }

        /// <summary>
        ///     Returns the average time taken plus an offset. We don't want the jobs to be running straight after one another now
        ///     do we.
        /// </summary>
        /// <returns></returns>
        public int Get()
        {
            lock (_listLock)
            {
                return Convert.ToInt32(Math.Floor(MinutesHistory.Average() + OffSet));
            }
        }
    }
}