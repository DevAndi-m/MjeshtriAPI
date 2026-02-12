using System.ComponentModel.DataAnnotations;

namespace MjeshtriAPI.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public User Client { get; set; }
        public User Expert { get; set; }

        public int ClientId { get; set; } 
        public int ExpertId { get; set; } 

        [Required]
        public string Description { get; set; } = string.Empty;

        // Statuses: "Pending", "Accepted", "Canceled", "Finished"

        [Required]
        public string Status { get; set; } = "Pending";

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        // Rating given by user after job is "Finished"
        public int? Rating { get; set; }
        public string? ReviewComment { get; set; }
    }
}
