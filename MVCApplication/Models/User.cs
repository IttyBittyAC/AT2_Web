namespace MVCApplication.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        //use a secure hashing algorithm to store password hashes, not plain text passwords
        //could also just store plain text password if we want to simplify it not sure
        public string PasswordHash { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }
    }
}
