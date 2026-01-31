using System.ComponentModel.DataAnnotations;

namespace MjeshtriAPI.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public int ClientId { get; set; } // The User who is hiring
        public int ExpertId { get; set; } // The Expert being hired

        [Required]
        public string Description { get; set; } = string.Empty;

        // Statuses: "Pending", "Accepted", "Refused", "Finished"
        public string Status { get; set; } = "Pending";

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        // Rating given by user after job is "Finished"
        public int? Rating { get; set; }
        public string? ReviewComment { get; set; }
    }
}
