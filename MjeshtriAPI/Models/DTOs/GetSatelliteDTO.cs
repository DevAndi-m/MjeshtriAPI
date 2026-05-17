namespace MjeshtriAPI.Models.DTOs
{
    public class GetSatelliteDTO
    {
        public int SatelliteId { get; set; }
        public string Name { get; set; } = "";
        public int PlanetId { get; set; }
        public string PlanetName { get; set; } = "";
    }
}
