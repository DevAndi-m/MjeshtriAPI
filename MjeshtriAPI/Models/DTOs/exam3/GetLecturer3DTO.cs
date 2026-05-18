namespace MjeshtriAPI.Models.DTOs.exam3
{
    public class GetLecturer3DTO
    {
        public int LecturerId { get; set; }
        public string LecturerName { get; set; } = "";
        public string Department { get; set; } = "";
        public string Email { get; set; } = "";
        public List<GetLecture3DTO> Lectures { get; set; } = new();
    }
}
