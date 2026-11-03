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
        private readonly IGenericRepository<Plan> _planRepo;
        public PlansController(IGenericRepository<Plan> planRepo)
        {
            _planRepo = planRepo;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planRepo.GetAllAsync(ct:ct);
            return View(plans);
        }
        public async Task<IActionResult> Details(int id,CancellationToken ct) {
            var plan = await _planRepo.GetByIdAsync(id, ct);
            if (plan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
    }
}
