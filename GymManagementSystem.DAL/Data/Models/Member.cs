using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Data.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }

        #region Relationships
        public HealthRecord HealthRecord { get; set; } = default!;
        public ICollection<Membership> Memberships { get; set; } = default!;
        public ICollection<Booking> Bookings { get; set; } = default!;
        #endregion
    }
}
