namespace LMS.EntityFramework
{
    using Abp.Domain.Entities;
    using Abp.EntityFramework;
    using Abp.EntityFramework.Repositories;

    public abstract class LMSRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<LMSDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected LMSRepositoryBase(IDbContextProvider<LMSDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class LMSRepositoryBase<TEntity> : LMSRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected LMSRepositoryBase(IDbContextProvider<LMSDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
