namespace MjeshtriAPI.Models.DTOs.exam3
{
    public class UpdateLecturer3DTO
    {
        public int LecturerId { get; set; }
        public string LecturerName { get; set; } = "";
        public string Department { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
