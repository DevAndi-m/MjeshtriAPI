using System.ComponentModel.DataAnnotations;

namespace MjeshtriAPI.Models
{
    public class Planet2
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public bool IsDeleted { get; set; } = false;
        public List<Satellite2> Satellites { get; set; } = new();
    }
}
