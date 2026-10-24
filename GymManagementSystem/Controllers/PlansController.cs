using GymManagementSystem.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Controllers
{
    public class PlansController : Controller
    {
        private readonly GymDbContext _DbContext;
        public PlansController()
        {
            _DbContext = new GymDbContext();
        }
        public async Task<IActionResult> Index()
        {
            var plans = await _DbContext.Plans.ToListAsync();
            return View(plans);
        }
        public async Task<IActionResult> Details(int id) {
            var plan = await _DbContext.Plans.FindAsync(id);
            if (plan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
    }
}
