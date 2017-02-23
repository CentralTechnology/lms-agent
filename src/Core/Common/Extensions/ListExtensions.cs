namespace Core.Common.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Abp.Domain.Entities;
    using LicenseMonitoringSystem.Core.Common.Portal.License.User;

    public static class ListExtensions
    {
        public static List<LicenseUser> ApplyUploadId(this List<LicenseUser> source, int uploadId)
        {
            return source.Select(u =>
            {
                u.UploadId = uploadId;
                return u;
            }).ToList();
        }

        public static List<TEntity> FilterExisting<TEntity, TPrimaryKey>(this List<TEntity> source, List<TEntity> comparison)
            where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            var destination = new List<TEntity>(source);

            if (comparison == null || comparison.Count == 0)
            {
                return destination;
            }

            destination.RemoveAll(d => !comparison.Any(c => c.Id.Equals(d.Id)));

            return destination;
        }

        public static List<TEntity> FilterExisting<TEntity>(this List<TEntity> source, List<TEntity> comparison)
            where TEntity : class, IEntity<int>, new()
        {
            return FilterMissing<TEntity, int>(source, comparison);
        }

        public static List<TEntity> FilterMissing<TEntity, TPrimaryKey>(this List<TEntity> source, List<TEntity> comparison)
            where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            var destination = new List<TEntity>(source);

            if (comparison == null || comparison.Count == 0)
            {
                return destination;
            }

            destination.RemoveAll(d => comparison.Any(c => c.Id.Equals(d.Id)));

            return destination;
        }

        public static List<TEntity> FilterMissing<TEntity>(this List<TEntity> source, List<TEntity> comparison)
            where TEntity : class, IEntity<int>, new()
        {
            return FilterMissing<TEntity, int>(source, comparison);
        }
    }
}