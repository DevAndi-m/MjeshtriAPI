using System.ComponentModel.DataAnnotations;

namespace MjeshtriAPI.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; } // Chat is linked to a specific job
        public int SenderId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
