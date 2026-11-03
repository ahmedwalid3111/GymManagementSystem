using GymManagementSystem.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Data.Configurations
{
    public class PlanConfigurations : IEntityTypeConfiguration<Plan>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Plan> builder)
        {
            
            builder.Property(p => p.Name).HasColumnType("varchar").HasMaxLength(50);
            builder.Property(p => p.Description).HasMaxLength(200);
            builder.Property(p => p.Price).HasPrecision(10, 2);
            builder.Property(p=>p.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.ToTable(TB=>TB.HasCheckConstraint("PlanDurationDaysCheck", "DurationDays Between 1 And 365"));

        }
    }
}
