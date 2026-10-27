using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Configurations
{
    public class MemberConfigurations: GymUserConfigurations<Member>,IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(M => M.CreatedAt).HasColumnName("JoinDate").HasDefaultValueSql("GETDATE()");
            base.Configure(builder);

        }
    }
}
