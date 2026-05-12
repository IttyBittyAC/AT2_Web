using System.ComponentModel.DataAnnotations;

namespace MVCApplication.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public int? EventId { get; set; }
        public string FullName { get; set; }

        public string Email { get; set; }

        public DateTime BookingDate { get; set; } 
    }
}
