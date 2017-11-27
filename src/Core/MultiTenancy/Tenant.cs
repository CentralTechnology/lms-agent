namespace LMS.MultiTenancy
{
    using Abp.MultiTenancy;
    using Authorization.Users;

    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {
            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}