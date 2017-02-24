namespace Core.Common.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Abp.Domain.Entities;
    using Models;

    public static class ListExtensions
    {
        public static List<LicenseUser> ApplyUploadId(this List<LicenseUser> source, int uploadId)
        {
            return source.Select(u =>
            {
                u.SupportUploadId = uploadId;
                return u;
            }).ToList();
        }

        public static List<LicenseUser> FilterExisting(this List<LicenseUser> source, List<LicenseUser> comparison)
        {
            var destination = new List<LicenseUser>(source);

            if (comparison == null || comparison.Count == 0)
            {
                return destination;
            }

            destination.RemoveAll(d => !comparison.Any(c => c.Id.Equals(d.Id)));

            return destination;
        }

        public static List<LicenseUser> FilterMissing(this List<LicenseUser> source, List<LicenseUser> comparison)
        {
            var destination = new List<LicenseUser>(source);

            if (comparison == null || comparison.Count == 0)
            {
                return destination;
            }

            destination.RemoveAll(d => comparison.Any(c => c.Id.Equals(d.Id)));

            return destination;
        }
    }
}