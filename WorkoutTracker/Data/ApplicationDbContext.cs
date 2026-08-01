using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;

namespace WorkoutTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WorkoutProgram> WorkoutPrograms { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Activity> Activities { get; set; }
    }
}