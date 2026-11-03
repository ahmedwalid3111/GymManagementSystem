using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Data.Models
{
    public class Membership : BaseEntity
    {
        public DateTime EndDate { get; set; }
        public string Status=> EndDate > DateTime.Now ? "Active" : "Expired";
        public bool IsActive => EndDate > DateTime.Now;
        #region Relationships
        public Member Member { get; set; } = default!;
        public int MemberId { get; set; }
        public Plan Plan { get; set; } = default!;
        public int PlanId { get; set; }

        #endregion
    }
}
