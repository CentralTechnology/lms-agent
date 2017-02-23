using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Core.Common
{
    using AutoMapper;
    using System.ComponentModel;
    using Abp.Timing;

    public class DateTimeConverter : ITypeConverter<DateTime, DateTimeOffset>
    {
        public DateTimeOffset Convert(DateTime source, DateTimeOffset destination, ResolutionContext context)
        {
            source = DateTime.SpecifyKind(source, DateTimeKind.Utc);

            DateTimeOffset offset = source;

            destination = offset;

            return destination;
        }
    }
}
