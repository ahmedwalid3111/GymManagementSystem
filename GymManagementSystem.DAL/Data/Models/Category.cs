using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Data.Models
{
    public class Category: BaseEntity
    {
        public string CategoryName { get; set; } = default!;
        #region Relationships
         public ICollection<Session> Sessions { get; set; } = default!;
        #endregion
    }
}
