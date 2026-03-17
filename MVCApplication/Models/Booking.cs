namespace MVCApplication.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public int EventId { get; set; }

        public DateTime BookingDate { get; set; }
    }
}
