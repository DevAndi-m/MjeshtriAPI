namespace MjeshtriAPI.Models.DTOs
{
    public class UpdateSatelliteDTO
    {
        public int SatelliteId { get; set; }
        public string Name { get; set; } = "";
        public int PlanetId { get; set; }
    }
}
