using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Controllers
{
    public class PlansController : Controller
    {
        //private readonly GymDbContext _DbContext;
        private readonly IPlanService _planService;
        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            
            return View(await _planService.GetAllPlansAsync(ct));
        }
        public async Task<IActionResult> Details(int id,CancellationToken ct) {
            var plan = await _planService.GetPlanByIdAsync(id, ct);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan not found";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
        [HttpGet]
        public async Task<IActionResult>Edit(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanToUpdateAsync(id, ct);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan can't be edited";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _planService.UpdatePlanAsync(id, model, ct);
            if (result)
            {
                TempData["SuccessMessage"] = "Plan updated successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Plan Failed to update";
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult>Activate(int id, CancellationToken ct)
        {
            var result = await _planService.ToggleActivationAsync(id, ct);
            if (result)
            {
                TempData["SuccessMessage"] = "Plan activation status toggled successfully";
            }
            TempData["ErrorMessage"] = "Failed to toggle plan activation status";
            
            return RedirectToAction(nameof(Index));
        }
    }
}
