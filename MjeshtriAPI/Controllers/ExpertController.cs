using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MjeshtriAPI.Data;

namespace MjeshtriAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpertController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ExpertController(ApplicationDbContext context)
        {
            _context = context;
        }

        // get categories of experts (distinkt), we can maybe fix by creating table for categories. We change this shit if we have time
        [HttpGet("categories")]
        public async Task<IActionResult> GetUniqueCategories()
        {
            var categories = await _context.Experts
                .Select(e => e.Category)
                .Distinct() 
                .Where(c => !string.IsNullOrEmpty(c))
                .ToListAsync();

            return Ok(categories);
        }


        // get all experts pipeline. 
        [HttpGet]
        public async Task<IActionResult> GetExperts(
            [FromQuery] string? categories,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var query = _context.Experts.Include(e => e.User).AsQueryable();

            if (!string.IsNullOrEmpty(categories)) 
            {
                var categoryList = categories.Split(',').ToList();
                query = query.Where(e => categoryList.Contains(e.Category));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(e => e.HourlyFee > minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(e => e.HourlyFee < maxPrice.Value);
            }

            var experts = await query.Select(e => new {
                e.Id,
                e.User.FullName,
                e.User.ProfilePictureUrl,
                e.Category,
                e.HourlyFee,
                e.AverageRating
            }).ToListAsync();

            return Ok(experts);
        }

        // get single expert.
        // get single expert.
        [HttpGet("{id}")]
        public async Task<IActionResult> getExpertById(int id)
        {
            var expert = await _context.Experts.Include(e => e.User).Where(e => e.Id == id).Select(e => new
            {
                e.Id,
                e.User.FullName,
                e.User.ProfilePictureUrl,
                e.User.Bio,
                e.Category,
                e.HourlyFee,
                e.AverageRating,
                e.Requirements,
                e.JobsTaken
            }).FirstOrDefaultAsync();

            if (expert == null)
                return NotFound(new { message = "Expert not found" });

            return Ok(expert);
        }
    }
}
