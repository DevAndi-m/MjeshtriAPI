namespace MjeshtriAPI.Models.DTOs
{
    public class UpdatePlayerDTO
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public int BirthYear { get; set; }
        public int TeamId { get; set; }
    }
}
