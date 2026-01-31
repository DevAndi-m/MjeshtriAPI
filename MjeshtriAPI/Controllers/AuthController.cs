using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MjeshtriAPI.Data;
using MjeshtriAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MjeshtriAPI.Models.DTOs;

namespace MjeshtriAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // 1. Check if user exists
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already registered.");

            // 2. Hash Password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 3. Create User
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = dto.IsExpert ? "Expert" : "User"
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // 4. If Expert, save to Experts table
                if (dto.IsExpert)
                {
                    var expert = new Expert
                    {
                        UserId = user.Id,
                        Category = dto.Category ?? "",
                        HourlyFee = dto.HourlyFee ?? 0,
                        Requirements = dto.Requirements ?? ""
                    };
                    _context.Experts.Add(expert);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok(new { message = "Registration successful!" });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred during registration.");
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var token = CreateToken(user);
            return Ok(new { token, role = user.Role, fullName = user.FullName });
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("userId", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}