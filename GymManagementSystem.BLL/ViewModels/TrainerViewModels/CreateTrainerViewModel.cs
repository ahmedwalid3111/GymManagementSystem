using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GymManagementSystem.DAL.Data.Models.Enums;
namespace GymManagementSystem.BLL.ViewModels.TrainerViewModels
{
    public class CreateTrainerViewModel
    {
        [Required(ErrorMessage ="Name is Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
        public string Name { get; set; } = default!;
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = default!;
        [Required(ErrorMessage = "Phone is Required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Invalid Phone Number")]

        public string Phone { get; set; } = default!;
        [Required(ErrorMessage = "Date of Birth is Required")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender is Required")]
        public Gender Gender { get; set; }
        [Required(ErrorMessage ="Building number is Required")]
        [Range(1,int.MaxValue, ErrorMessage = "Building Number must be higher than 0")]
        public int BuildingNumber { get; set; }
        [Required(ErrorMessage ="City is Required")]
        [StringLength(100,MinimumLength =2, ErrorMessage ="City must be between 2 and 100 characters")]
        public string City { get; set; } = default!;

        [Required(ErrorMessage ="Street is Required")]
        [StringLength(150,MinimumLength =2, ErrorMessage ="Street must be between 2 and 150 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Street can only contain letters, numbers, and spaces")]
        public string Street { get; set; } = default!;

        [Required(ErrorMessage ="Speciality is Required")]
        [EnumDataType(typeof(Specialities))]
        public Specialities Specialities { get; set; } = default!;  

    }
}
