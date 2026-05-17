using System.ComponentModel.DataAnnotations;

namespace MjeshtriAPI.Models
{
    public class Planet
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public bool IsDeleted { get; set; } = false;
        public List<Satellite> Satellites { get; set; } = new();
    }
}
