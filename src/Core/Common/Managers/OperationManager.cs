namespace LMS.Common.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abp.Dependency;
    using Abp.Domain.Services;

    public class OperationManager : DomainService, ISingletonDependency
    {
        private readonly object _listLock = new object();
        protected internal List<int> MinutesHistory { get; private set; }

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

        public int Get()
        {
            lock (_listLock)
            {
                return (int) Math.Floor(MinutesHistory.Average());
            }
        }
    }
}