using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MjeshtriAPI.Data;
using MjeshtriAPI.Models;
using MjeshtriAPI.Models.DTOs;

namespace MjeshtriAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class testController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        public testController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("post-team")]
        public async Task<IActionResult> Post([FromBody] CreateTeamDTO team)
        {
            var teamEntity = new Team
            {
                TeamId = team.TeamId,
                Name = team.Name
            };

            _context.Teams.Add(teamEntity);

            await _context.SaveChangesAsync();

            return Ok(new { message = "good" });
        }

        [HttpPost("post-player")]
        public async Task<IActionResult> PostPlayers([FromBody] PostPlayersDTO player)
        {
            var playerEntity = new Player
            {
                PlayerId = player.PlayerId,
                Name = player.Name,
                Number = player.Number,
                BirthYear = player.BirthYear,
                TeamId = player.TeamId
            };

            _context.Players.Add(playerEntity);
            await _context.SaveChangesAsync();
            return Ok(new { message = "good" });
        }

        [HttpGet("players")]
        public async Task<IActionResult> Get()
        {
            var players = await _context.Players
                .Select(u => new
                {
                   u.PlayerId,
                    u.Name,
                    u.Number,
                    u.BirthYear,
                    TeamName = u.Team.Name
                })
                .ToListAsync();

            return Ok(players);
        }

        [HttpDelete("delete-player")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            _context.Players.Remove(new Player { PlayerId = id });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Player deleted successfully" });
        }

        [HttpPut("update-player     ")]
        public async Task<IActionResult> UpdatePlayer([FromBody] UpdatePlayerDTO player)
        {
                       var playerEntity = await _context.Players.FindAsync(player.PlayerId);
            if (playerEntity == null)
            {
                return NotFound(new { message = "Player not found" });
            }
            playerEntity.Name = player.Name;
            playerEntity.Number = player.Number;
            playerEntity.BirthYear = player.BirthYear;
            playerEntity.TeamId = player.TeamId;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Player updated successfully" });
        }
    }
}
