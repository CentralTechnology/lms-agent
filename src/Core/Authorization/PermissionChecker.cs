namespace LMS.Authorization
{
    using Abp.Authorization;
    using MultiTenancy;
    using Roles;
    using Users;

    public class PermissionChecker : PermissionChecker<Tenant,Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
