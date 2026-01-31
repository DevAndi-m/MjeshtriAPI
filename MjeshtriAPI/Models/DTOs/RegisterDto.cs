namespace MjeshtriAPI.Models.DTOs
{
    public class RegisterDto
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public bool IsExpert { get; set; }
        public string? Category { get; set; }
        public decimal? HourlyFee { get; set; }
        public string? Requirements { get; set; }
    }
}
