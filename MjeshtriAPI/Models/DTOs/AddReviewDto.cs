namespace MjeshtriAPI.Models.DTOs
{
    public class AddReviewDto
    {
        public int BookingId { get; set; }

        public int Rating { get; set; }

        public string ReviewComment { get; set; } = string.Empty;
    }
}
