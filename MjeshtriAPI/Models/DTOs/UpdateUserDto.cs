namespace MjeshtriAPI.Models.DTOs
{
    public class UpdateUserDto
    {
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Category { get; set; }
        public decimal? HourlyFee { get; set; }
        public string? Requirements { get; set; }
    }
}