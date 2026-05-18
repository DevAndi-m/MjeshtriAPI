using Microsoft.EntityFrameworkCore;
using MjeshtriAPI.Models;
using MjeshtriAPI.Models.Exam1;
using MjeshtriAPI.Models.Exam3;

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
        // exam
        public DbSet<Planet> Planets { get; set; }
        public DbSet<Satellite> Satellites { get; set; }

        // exam 1
        public DbSet<Employee> Employees { get; set; } 
        public DbSet<Contract> Contracts { get; set; }

        // exam 2
        public DbSet<Planet2> Planets2 { get; set; }
        public DbSet<Satellite2> Satellites2 { get; set; }

        // exam 3
        public DbSet<Lecturer3> Lecturers3 { get; set; }
        public DbSet<Lecture3> Lectures3 { get; set; }
    }
}
