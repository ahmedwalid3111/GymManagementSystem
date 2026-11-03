using GymManagementSystem.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Data.Configurations
{
    public class BookingConfigurations: IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Ignore(B => B.Id);
            builder.HasKey(B => new { B.MemberId, B.SessionId });
            builder.Property(B=>B.CreatedAt).HasColumnName("BookingDate").HasDefaultValueSql("GETDATE()");
        }
    

    }
}
