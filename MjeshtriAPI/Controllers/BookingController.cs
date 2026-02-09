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
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] string newStatus)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int currentUserId = int.Parse(userIdClaim);

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound("Booking not found.");

            var expert = await _context.Experts.FirstOrDefaultAsync(e => e.Id == booking.ExpertId);

            if (expert == null || expert.UserId != currentUserId)
            {
                return Forbid();
            }

            bool isValid = false;

            switch (booking.Status)
            {
                case "Pending":
                    if (newStatus == "Accepted" || newStatus == "Canceled") isValid = true;
                    break;
                case "Accepted":
                    if (newStatus == "Finished" || newStatus == "Canceled") isValid = true;
                    break;
                default:
                    return BadRequest($"Cannot change status from {booking.Status} to {newStatus}.");
            }

            if (!isValid)
            {
                return BadRequest($"Invalid status transition from {booking.Status} to {newStatus}.");
            }

            booking.Status = newStatus;

            if (newStatus == "Finished")
            {
                expert.JobsTaken++;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Booking status updated to {newStatus}." });
        }


        [HttpPatch("review")]
        public async Task<IActionResult> SubmitReview([FromBody] AddReviewDto dto)
        {
            // 1. Get Logged-in User ID
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var booking = await _context.Bookings.FindAsync(dto.BookingId);

            if (booking == null) return NotFound("Booking not found.");

            if (booking.ClientId != currentUserId)
            {
                return Forbid("You can only review jobs that you personally booked.");
            }

            if (booking.Status != "Finished")
            {
                return BadRequest("You can only review a job once it is marked as Finished.");
            }

            if (booking.Rating != null)
            {
                return BadRequest("You have already submitted a review for this job.");
            }

            if (dto.Rating < 1 || dto.Rating > 10)
            {
                return BadRequest("Rating must be between 1 and 10.");
            }

            if (dto.ReviewComment.Length > 100)
            {
                return BadRequest("Review comment cannot exceed 100 characters.");
            }

            booking.Rating = dto.Rating;
            booking.ReviewComment = dto.ReviewComment;

            await UpdateExpertAverageRating(booking.ExpertId);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Review submitted successfully!" });
        }

        private async Task UpdateExpertAverageRating(int expertId)
        {
            var expert = await _context.Experts.FindAsync(expertId);
            if (expert != null)
            {
                var ratings = await _context.Bookings
                    .Where(b => b.ExpertId == expertId && b.Rating != null)
                    .Select(b => b.Rating.Value)
                    .ToListAsync();

                if (ratings.Any())
                {
                    expert.AverageRating = Math.Round(ratings.Average(), 1);
                }
            }
        }

        [HttpPost("hire")]
        public async Task<IActionResult> HireExpert([FromBody] CreateBookingDto dto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User ID not found in token");
            }

            int clientId = int.Parse(userIdClaim);

            var expert = await _context.Experts.FindAsync(dto.ExpertId);

            if (expert == null)
            {
                return NotFound("Expert you are trying to hire does not exist");
            }

            if (expert.UserId == clientId)
            {
                return BadRequest("You cannot hire yourself as an expert");
            }

            var existingBooking = await _context.Bookings.Where(b => b.ClientId == clientId && b.ExpertId == expert.UserId).Where(b => b.Status == "Pending" || b.Status == "Accepted").FirstOrDefaultAsync();

            if (existingBooking != null)
            {
                return BadRequest($"You already have booking with this expert where status is {existingBooking.Status.ToLower()}");
            }

            var booking = new Booking
            {
                ClientId = clientId,
                ExpertId = dto.ExpertId,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Booking created successfully", bookingId = booking.Id });
        }
    }
}
