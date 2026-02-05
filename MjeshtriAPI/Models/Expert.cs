using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MjeshtriAPI.Models
{
    public class Expert
    {
        [Key]
        public int Id { get; set; }

        // Links back to the User table
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public string Category { get; set; } = string.Empty; 
        public decimal HourlyFee { get; set; }
        public string Bio { get; set; } = string.Empty;
        public int JobsTaken { get; set; } = 0;
        public double AverageRating { get; set; } = 0.0;
        public bool IsPublic { get; set; } = true;

        // store requirements as a string
        // You can split these by a comma in the frontend
        public string Requirements { get; set; } = string.Empty;
    }
}
