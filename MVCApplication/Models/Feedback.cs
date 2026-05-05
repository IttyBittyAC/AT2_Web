using System.ComponentModel.DataAnnotations;

namespace MVCApplication.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Heading { get; set; } = string.Empty;


        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public bool WantsContact { get; set; } = false;
    }
}
