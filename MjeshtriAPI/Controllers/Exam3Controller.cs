using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MjeshtriAPI.Data;
using MjeshtriAPI.Models.DTOs.exam3;
using MjeshtriAPI.Models.Exam3;

namespace MjeshtriAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Exam3Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Exam3Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-lecturer")]
        public async Task<IActionResult> CreateLecturer(CreateLecturer3DTO dto)
        {
            Lecturer3 l = new Lecturer3
            {
                LecturerName = dto.LecturerName,
                Department = dto.Department,
                Email = dto.Email
            };

            _context.Lecturers3.Add(l);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Lecturer created with ID: {l.LecturerId}" });
        }

        [HttpPost("create-lecture")]
        public async Task<IActionResult> CreateLecture(CreateLecture3DTO dto)
        {
            Lecture3 lec = new Lecture3
            {
                LectureName = dto.LectureName,
                LecturerId = dto.LecturerId
            };

            _context.Lectures3.Add(lec);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Lecture created with ID: {lec.LectureId}" });
        }

        [HttpGet("get-lecturers")]
        public async Task<IActionResult> GetLecturers()
        {
            var lecturers = await _context.Lecturers3
                .Include(l => l.Lectures)
                .ToListAsync();

            return Ok(lecturers.Select(l => new GetLecturer3DTO
            {
                LecturerId = l.LecturerId,
                LecturerName = l.LecturerName,
                Department = l.Department,
                Email = l.Email,
                Lectures = l.Lectures.Select(lec => new GetLecture3DTO
                {
                    LectureId = lec.LectureId,
                    LectureName = lec.LectureName
                }).ToList()
            }));
        }

        [HttpPut("update-lecturer")]
        public async Task<IActionResult> UpdateLecturer(UpdateLecturer3DTO dto)
        {
            var lecturer = await _context.Lecturers3.FirstOrDefaultAsync(l => l.LecturerId == dto.LecturerId);

            if (lecturer == null)
                return NotFound(new { message = $"Lecturer with ID: {dto.LecturerId} not found." });

            lecturer.LecturerName = dto.LecturerName;
            lecturer.Department = dto.Department;
            lecturer.Email = dto.Email;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Lecturer with ID: {dto.LecturerId} updated successfully." });
        }

        [HttpDelete("delete-lecture/{id}")]
        public async Task<IActionResult> DeleteLecture(int id)
        {
            var lecture = await _context.Lectures3.FirstOrDefaultAsync(l => l.LectureId == id);

            if (lecture == null)
                return NotFound(new { message = $"Lecture with ID: {id} not found." });

            _context.Lectures3.Remove(lecture);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Lecture with ID: {id} deleted successfully." });
        }
    }
}
