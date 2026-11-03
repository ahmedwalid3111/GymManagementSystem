using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Data.Models
{
    public class Booking: BaseEntity
    {
        #region Relationships
        public Member Member { get; set; } = default!;
        public int MemberId { get; set; }
        public Session Session { get; set; } = default!;
        public int SessionId { get; set; }
        #endregion
        public bool IsAttended { get; set; }
    }
}
