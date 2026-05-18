using System.ComponentModel.DataAnnotations;

namespace MjeshtriAPI.Models.Exam3
{
    public class Lecture3
    {
        [Key]
        public int LectureId { get; set; }
        public string LectureName { get; set; } = "";
        public int LecturerId { get; set; }
        public Lecturer3 Lecturer { get; set; } = null!;
    }
}
