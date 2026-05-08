namespace MVCApplication.Helpers
{
    public static class SessionKeys
    {
        public const string Type = "user";
        public const string Role = "role";
    }
    public enum UserRole
    {
        user,
        admin
    }
}
