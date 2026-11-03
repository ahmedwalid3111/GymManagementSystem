using GymManagementSystem.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Data.Configurations
{
    public class MembershipConfigurations:IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.HasKey(M=>M.Id);
            builder.Property(M => M.CreatedAt).HasColumnName("StartDate").HasDefaultValueSql("GETDATE()");
        }

    }
}
