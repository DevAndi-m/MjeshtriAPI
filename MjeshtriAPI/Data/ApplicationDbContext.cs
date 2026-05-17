using Microsoft.EntityFrameworkCore;
using MjeshtriAPI.Models;
using MjeshtriAPI.Models.Exam1;

namespace MjeshtriAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Expert> Experts { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Planet> Planets { get; set; }
        public DbSet<Satellite> Satellites { get; set; }

        // exam 1
        public DbSet<Employee> Employees { get; set; } 
        public DbSet<Contract> Contracts { get; set; }



    }
}
