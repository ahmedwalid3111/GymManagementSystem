using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Configurations
{
    public class SessionConfigurations : IEntityTypeConfiguration<Session>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Session> builder)
        {
            builder.ToTable(TB =>
            {
                TB.HasCheckConstraint("CapacityCheck", "Capacity between 1 and 25");
                TB.HasCheckConstraint("DateCheck", "EndDate > StartDate");
            });
        }
    }
}
