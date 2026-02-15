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
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

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

            if (!string.IsNullOrEmpty(dto.ReviewComment) && dto.ReviewComment.Length > 100)
            {
                return BadRequest("Review comment cannot exceed 100 characters.");
            }

            booking.Rating = dto.Rating;
            booking.ReviewComment = dto.ReviewComment ?? string.Empty;

            // Persist the booking update first so the rating exists in the DB for aggregation
            await _context.SaveChangesAsync();

            // Recalculate and persist the expert's average rating based on saved ratings
            await UpdateExpertAverageRating(booking.ExpertId);
            await _context.SaveChangesAsync();

            var expert = await _context.Experts.FindAsync(booking.ExpertId);
            var avg = expert?.AverageRating;

            return Ok(new { message = "Review submitted successfully!", averageRating = avg, expertId = booking.ExpertId });
        }

        private async Task UpdateExpertAverageRating(int expertId)
        {
            var expert = await _context.Experts.FindAsync(expertId);
            if (expert != null)
            {
                var ratings = await _context.Bookings
                    .Where(b => b.ExpertId == expertId && b.Rating != null)
                    .Select(b => (double?)b.Rating)
                    .ToListAsync();

                var numericRatings = ratings.Where(r => r.HasValue).Select(r => r.Value).ToList();

                if (numericRatings.Any())
                {
                    expert.AverageRating = Math.Round(numericRatings.Average(), 1);
                }
                else
                {
                    expert.AverageRating = 0;
                }
            }
        }

        // Public endpoint to fetch expert reviews (ratings + comments)
        [AllowAnonymous]
        [HttpGet("expert-reviews/{expertId}")]
        public async Task<IActionResult> GetExpertReviews(int expertId)
        {
            var reviews = await _context.Bookings
                .Where(b => b.ExpertId == expertId && b.Rating != null)
                .OrderByDescending(b => b.RequestedAt)
                .Select(b => new
                {
                    bookingId = b.Id,
                    clientId = b.ClientId,
                    rating = b.Rating,
                    reviewComment = b.ReviewComment,
                    requestedAt = b.RequestedAt
                })
                .ToListAsync();

            var result = new List<object>();
            foreach (var r in reviews)
            {
                var clientUser = await _context.Users.FindAsync((int)r.clientId);
                result.Add(new
                {
                    bookingId = r.bookingId,
                    clientId = r.clientId,
                    clientName = clientUser != null ? clientUser.FullName : null,
                    rating = r.rating,
                    reviewComment = r.reviewComment,
                    requestedAt = r.requestedAt
                });
            }

            return Ok(result);
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

            var existingBooking = await _context.Bookings
                .Where(b => b.ClientId == clientId && b.ExpertId == expert.UserId)
                .Where(b => b.Status == "Pending" || b.Status == "Accepted")
                .FirstOrDefaultAsync();

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
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetUserBookings()
        {
            var userIdClaim = this.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var expertRecord = await _context.Experts.FirstOrDefaultAsync(e => e.UserId == currentUserId);

            List<Booking> bookings;
            if (expertRecord != null)
            {
                bookings = await _context.Bookings
                    .Where(b => b.ClientId == currentUserId || b.ExpertId == expertRecord.Id)
                    .OrderByDescending(b => b.RequestedAt)
                    .ToListAsync();
            }
            else
            {
                bookings = await _context.Bookings
                    .Where(b => b.ClientId == currentUserId)
                    .OrderByDescending(b => b.RequestedAt)
                    .ToListAsync();
            }

            var result = new List<object>();

            foreach (var booking in bookings)
            {
                var expert = await _context.Experts.FindAsync(booking.ExpertId);
                var expertUser = expert != null ? await _context.Users.FindAsync(expert.UserId) : null;
                var clientUser = await _context.Users.FindAsync(booking.ClientId);

                result.Add(new
                {
                    booking.Id,
                    booking.ExpertId,
                    booking.ClientId,
                    booking.Description,
                    booking.Status,
                    booking.RequestedAt,
                    booking.Rating,
                    booking.ReviewComment,
                    ExpertName = !string.IsNullOrEmpty(expertUser?.FullName) ? expertUser.FullName : $"Expert #{expert?.UserId ?? booking.ExpertId}",
                    ClientName = !string.IsNullOrEmpty(clientUser?.FullName) ? clientUser.FullName : $"Client #{booking.ClientId}",
                    ExpertBio = expertUser?.Bio,
                    ClientBio = clientUser?.Bio,
                    Role = booking.ClientId == currentUserId ? "Client" : "Expert"
                });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            int currentUserId = int.Parse(userIdClaim);

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound("Booking not found.");

            if (booking.ClientId != currentUserId)
            {
                return Forbid("You can only cancel your own bookings.");
            }

            if (booking.Status != "Pending")
            {
                return BadRequest($"You can only cancel bookings with Pending status. Current status: {booking.Status}");
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Booking cancelled and deleted successfully." });
        }
    }
}