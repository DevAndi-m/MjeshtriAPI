using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MjeshtriAPI.Models
{
    public class Satellite2
    {
        [Key]
        public int SatelliteId { get; set; }
        public string Name { get; set; } = "";
        public bool IsDeleted { get; set; }
        public int PlanetId { get; set; }
        public Planet2 Planet { get; set; } = null!;
    }
}
