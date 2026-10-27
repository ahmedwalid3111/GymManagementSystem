using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    internal class HealthRecord:BaseEntity
    {
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Note { get; set; } = default!;
        public string BloodType { get; set; } = default!;
    }
}
