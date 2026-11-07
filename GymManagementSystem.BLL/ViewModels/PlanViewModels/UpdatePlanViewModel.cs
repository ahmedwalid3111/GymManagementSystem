using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.PlanViewModels
{
    public class UpdatePlanViewModel
    {
        public string PlanName { get; set; } = default!;
        [Required(ErrorMessage = "Description is Required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 200")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Duration is Required")]
        [Range(1,365,ErrorMessage = "Duration must be between 1 and 365")]
        public int DurationDays { set; get; }

        [Required(ErrorMessage = "Price is Required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { set; get; }


    }
}
