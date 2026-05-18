using System.ComponentModel.DataAnnotations;

namespace MjeshtriAPI.Models.Exam3
{
    public class Lecturer3
    {
        [Key]
        public int LecturerId { get; set; }
        public string LecturerName { get; set; } = "";
        public string Department { get; set; } = "";
        public string Email { get; set; } = "";
        public List<Lecture3> Lectures { get; set; } = new();
    }
}
