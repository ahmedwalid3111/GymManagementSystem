using GymManagementSystem.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Data.Models
{
    public class Trainer:GymUser
    {
        public Specialities Speciality { get; set; }
        #region Relationships
        public ICollection<Session> Sessions { get; set; } = default!;

        #endregion
    }
}
