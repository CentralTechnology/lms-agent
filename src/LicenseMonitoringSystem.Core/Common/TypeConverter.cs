namespace Core.Common
{
    using System;
    using AutoMapper;

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