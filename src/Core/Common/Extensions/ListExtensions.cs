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
                u.ManagedSupportId = uploadId;
                return u;
            }).ToList();
        }

        public static List<TEntity> FilterCreate<TEntity>(this List<TEntity> source, List<TEntity> comparison)
            where TEntity : class, IEntity
        {
            return source.FilterCreate<TEntity, int>(comparison);
        }

        public static List<TEntity> FilterCreate<TEntity, TPrimaryKey>(this List<TEntity> source, List<TEntity> comparison)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var newSource = new List<TEntity>(source);

            return comparison.Where(s => !newSource.Any(c => c.Id.Equals(s.Id))).ToList();
        }

        public static List<TEntity> FilterDelete<TEntity, TPrimaryKey>(this List<TEntity> source, List<TEntity> comparison)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var newSource = new List<TEntity>(source);

            return newSource.Where(s => !comparison.Any(c => c.Id.Equals(s.Id))).ToList();
        }

        public static List<TEntity> FilterDelete<TEntity>(this List<TEntity> source, List<TEntity> comparison)
            where TEntity : class, IEntity
        {
            return source.FilterDelete<TEntity, int>(comparison);
        }

        public static List<TEntity> FilterUpdate<TEntity, TPrimaryKey>(this List<TEntity> source, List<TEntity> comparison)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var newSource = new List<TEntity>(source);

            return comparison.Where(c => newSource.Any(s => s.Id.Equals(c.Id))).ToList();
        }

        public static List<TEntity> FilterUpdate<TEntity>(this List<TEntity> source, List<TEntity> comparison)
            where TEntity : class, IEntity
        {
            return source.FilterUpdate<TEntity, int>(comparison);
        }
    }
}