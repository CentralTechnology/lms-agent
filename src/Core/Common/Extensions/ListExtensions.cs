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

        public static List<LicenseUser> FilterUpdate(this List<LicenseUser> source, List<LicenseUser> comparison)
        {
            List<LicenseUser> newSource = new List<LicenseUser>(source);

            return comparison.Where(c => newSource.Any(s => s.Id.Equals(c.Id))).ToList();
        }

        public static List<LicenseUser> FilterCreate(this List<LicenseUser> source, List<LicenseUser> comparison)
        {
            List<LicenseUser> newSource = new List<LicenseUser>(source);

            return comparison.Where(s => !newSource.Any(c => c.Id.Equals(s.Id))).ToList();
        }

        public static List<LicenseUser> FilterDelete(this List<LicenseUser> source, List<LicenseUser> comparison)
        {
            List<LicenseUser> newSource = new List<LicenseUser>(source);

            return newSource.Where(s => !comparison.Any(c => c.Id.Equals(s.Id))).ToList();
        }
    }
}