namespace Core.Factory
{
    using Users;

    public static class UserFactory
    {
        public static UserManager UserManager()
        {
            return new UserManager();
        }
    }
}