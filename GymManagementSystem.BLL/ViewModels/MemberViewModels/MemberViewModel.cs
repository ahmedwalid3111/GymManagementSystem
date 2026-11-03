using GymManagementSystem.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.MemberViewModels
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }= default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Photo { get; set; } = default!;
        public Gender Gender { get; set; }
    }
}
