using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MjeshtriAPI.Data;
using MjeshtriAPI.Models;
using MjeshtriAPI.Models.DTOs;

namespace MjeshtriAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExamController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-planet")]
        public async Task<IActionResult> CreatePlanet(CreatePlanetDTO planet)
        {
            Planet p = new Planet
            {
                Name = planet.Name,
                Type = planet.Type
            };

            _context.Planets.Add(p);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Planet Created Successfully with ID: {p.Id}"
            });
        }

        [HttpPost("create-satellite")]
        public async Task<IActionResult> CreateSatellite(CreateSatelliteDTO satellite)
        {
            Satellite s = new Satellite
            {
                Name = satellite.Name,
                PlanetId = satellite.PlanetId
            };

            _context.Satellites.Add(s);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Satellite Created Successfully with ID: {s.SatelliteId}"
            });
        }

        [HttpGet("get-planets")]
        public async Task<IActionResult> GetPlanets()
        {
            var planets = await _context.Planets.ToListAsync();

            return Ok(planets);
        }

        [HttpGet("get-satellites")]
        public async Task<IActionResult> GetSatellites()
        {
            var satellites = await _context.Satellites
                .Where(s => s.IsDeleted == false)
                .Include(s => s.Planet)
                .Select(s => new GetSatelliteDTO
                {
                    SatelliteId = s.SatelliteId,
                    Name = s.Name,
                    PlanetName = s.Planet.Name,
                    PlanetId = s.PlanetId

                })
                .ToListAsync();

            return Ok(satellites);
        }

        [HttpPut("update-satelite")]
        public async Task<IActionResult> UpdateSatellite(UpdateSatelliteDTO sat)
        {
            var satellite = await _context.Satellites.FirstOrDefaultAsync(s => s.SatelliteId == sat.SatelliteId);

            if (satellite == null)
            {
                return NotFound(new
                {
                    message = $"Satellite with ID: {sat.SatelliteId} not found."
                });
            }

            satellite.Name = sat.Name;
            satellite.PlanetId = sat.PlanetId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Satellite with ID: {sat.SatelliteId} updated successfully."
            });
        }

        [HttpDelete("delete-satellite/{id}")]
        public async Task<IActionResult> DeleteSatellite(int id)
        {
            var satellite = await _context.Satellites.FirstOrDefaultAsync(s => s.SatelliteId == id);
            if (satellite == null)
            {
                return NotFound(new
                {
                    message = $"Satellite with ID: {id} not found."
                });
            }
            satellite.IsDeleted = true;
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = $"Satellite with ID: {id} deleted successfully."
            });
        }
    }
}
