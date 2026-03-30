using System.ComponentModel.DataAnnotations;

namespace MVCApplication.Models
{
    public class Booking
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "FullName is required")]
        [Display(Name = "Fullname")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Preferred Date")]
        public DateTime BookingDate { get; set; } 
    }
}
