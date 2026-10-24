using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Models;
using GymManagementSystem.Configurations;
namespace GymManagementSystem.DbContexts
{
    public class GymDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure the database connection string here
            optionsBuilder.UseSqlServer("Server=.;Database=GymDB;Trusted_Connection=true; TrustServerCertificate=true;");

        }
        public DbSet<Plan> Plans { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PlanConfigurations());
        }
    }
}
