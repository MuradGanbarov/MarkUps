using MarkUps.Areas.Admin.Models.Utilities.Enums;
using MarkUps.Areas.Admin.ViewModels;
using MarkUps.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MarkUps.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = new()
            {
                Name = vm.Name,
                Surname = vm.Surname,
                UserName = vm.UserName,
                Email = vm.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(user,vm.Password);
            if(!result.Succeeded)
            {
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    
                }
                return View();
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home", new { Area = "" });

        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new {Area=""});
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByEmailAsync(vm.UserNameOrEmail);
            if(user is null)
            {
                user = await _userManager.FindByNameAsync(vm.UserNameOrEmail);
                if(user is null)
                {
                    ModelState.AddModelError(string.Empty, "Password,Email or Username is incorrect");
                    return View();
                }
            }
            var result = await _signInManager.PasswordSignInAsync(user, vm.Password,vm.IsRemember,true);
            if(result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "We have maintance,please try later");
                return View();
            }
            if(!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Password,Email or Username is incorrect");
                return View();
            }
            return RedirectToAction("Index", "Home", new { Area = "" });


        }

        public async Task<IActionResult> CreateRoles()
        {
            foreach(UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if(!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString(),
                    });
                }
            }
            return RedirectToAction("Index", "Home", new { Area = "" });
        }
    }
}
