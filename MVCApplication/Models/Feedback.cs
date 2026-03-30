using System.ComponentModel.DataAnnotations;

namespace MVCApplication.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Heading is required")]
        [StringLength(200, ErrorMessage = "Heading cannot exceed 200 characters")]
        [Display(Name = "Heading")]
        public string Heading { get; set; } = string.Empty;


        [Required(ErrorMessage = "FullName is required")]
        [Display(Name = "Fullname")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a message")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters")]
        [MaxLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a feedback type")]
        [StringLength(50)]
        [Display(Name = "Feedback Type")]
        public string Type { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Display(Name = "Would you like us to contact you?")]
        public bool WantsContact { get; set; } = false;
    }
}
