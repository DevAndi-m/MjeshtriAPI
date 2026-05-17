using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MjeshtriAPI.Models
{
    public class Player
    {
        [Required]
        public int PlayerId { get; set; }
        public int TeamId { get; set; }

        public string Name { get; set; }
        public int Number { get; set; }
        public int BirthYear { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }
    }
}
