using System.ComponentModel.DataAnnotations;

namespace MVCApplication.Models
{
    public class User
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "FullName is required")]
        [Display(Name = "UserName")]
        public string? Username { get; set; }

        //use a secure hashing algorithm to store password hashes, not plain text passwords
        //could also just store plain text password if we want to simplify it not sure
        public string? PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "FullName is required")]
        [Display(Name = "Fullname")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string? Email { get; set; }
        public string? Role { get; set; } = "user";
    }
}
