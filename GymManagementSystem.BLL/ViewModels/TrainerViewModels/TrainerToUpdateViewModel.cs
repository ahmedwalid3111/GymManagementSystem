using GymManagementSystem.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.TrainerViewModels
{
    public class TrainerToUpdateViewModel
    {
        public string Name { get; set; } = default!;

        public string Email { get; set; } = default!;

        // Editable Properties
        [Required(ErrorMessage = "Phone is Required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$",
            ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; } = default!;

        [Required(ErrorMessage = "Building number is Required")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Building Number must be higher than 0")]
        public int BuildingNumber { get; set; }

        [Required(ErrorMessage = "City is Required")]
        [StringLength(100, MinimumLength = 2)]
        public string City { get; set; } = default!;

        [Required(ErrorMessage = "Street is Required")]
        [StringLength(150, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$")]
        public string Street { get; set; } = default!;

        [Required(ErrorMessage = "Speciality is Required")]
        [EnumDataType(typeof(Specialities))]
        public Specialities Specialities { get; set; }
    }
}
