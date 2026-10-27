using GymManagementSystem.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    public abstract class GymUser : BaseEntity
    {
        public string Name { get; set; }= default!;
        public string Email { get; set; }= default!;
        public string Phone { get; set; }= default!;
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public Address Address { get; set; } = default!;
    }
}
