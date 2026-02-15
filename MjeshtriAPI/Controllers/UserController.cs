using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MjeshtriAPI.Data;
using MjeshtriAPI.Models;
using MjeshtriAPI.Models.DTOs;
using System.Security.Claims;

namespace MjeshtriAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET current user profile
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null) return NotFound("User not found.");

            var expert = await _context.Experts.FirstOrDefaultAsync(e => e.UserId == currentUserId);

            var result = new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Role,
                user.Bio,
                user.ProfilePictureUrl,
                Category = expert?.Category ?? "",
                HourlyFee = expert?.HourlyFee ?? 0,
                Requirements = expert?.Requirements ?? "",
                AverageRating = expert?.AverageRating,
                JobsTaken = expert?.JobsTaken ?? 0
            };

            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null) return NotFound("User not found.");

            if (string.IsNullOrEmpty(dto.Password) || dto.Password.Length < 8)
            {
                return BadRequest("Password must be at least 8 characters long.");
            }

            if (!dto.Password.Any(char.IsDigit) || !dto.Password.Any(char.IsLetter))
            {
                return BadRequest("Password must contain at least one letter and one digit.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while changing the password.", error = ex.Message });
            }
        }

        // PUT update current user profile
        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUserProfile([FromBody] UpdateUserDto dto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null) return NotFound("User not found.");

            // Update basic user info
            if (!string.IsNullOrEmpty(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrEmpty(dto.Bio))
                user.Bio = dto.Bio;

            if (!string.IsNullOrEmpty(dto.ProfilePictureUrl))
                user.ProfilePictureUrl = dto.ProfilePictureUrl;

            // If user is an expert, update expert info
            if (user.Role == "Expert")
            {
                var expert = await _context.Experts.FirstOrDefaultAsync(e => e.UserId == currentUserId);
                if (expert != null)
                {
                    if (!string.IsNullOrEmpty(dto.Category))
                        expert.Category = dto.Category;

                    if (dto.HourlyFee.HasValue && dto.HourlyFee > 0)
                        expert.HourlyFee = dto.HourlyFee.Value;

                    if (!string.IsNullOrEmpty(dto.Requirements))
                        expert.Requirements = dto.Requirements;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Profile updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the profile.", error = ex.Message });
            }
        }
    }
}