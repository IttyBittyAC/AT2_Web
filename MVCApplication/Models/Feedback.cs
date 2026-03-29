namespace MVCApplication.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Message { get; set; }

        public DateTime SubmittedDate { get; set; }
    }
}
