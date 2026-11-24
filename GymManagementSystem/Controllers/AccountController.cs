using GymManagementSystem.BLL.ViewModels.AccountViewModels;
using GymManagementSystem.Controllers;
using GymManagementSystem.DAL.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager ,ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login( LoginViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user =await _userManager.FindByEmailAsync(model.Email);

            if(user is null)
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email or Password");
                return View(model);
            }
            var result= await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.Id} Signed In Successfully");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            if (result.IsLockedOut)
            {
                _logger.LogInformation($"User {user.Id} is locked out");
                ModelState.AddModelError("InvalidLogin", "This account is temporarily locked, try again later");
            }else if (result.IsNotAllowed)
            {
                ModelState.AddModelError("InvalidLogin", "Sign in is not allowed for this account");
            }
            else
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email or Password");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
