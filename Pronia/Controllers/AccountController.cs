using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Models;
using Pronia.ViewModels.User;

namespace Pronia.Controllers
{
    public class AccountController : Controller
    {
        UserManager<AppUser> _userManager { get; }
        SignInManager<AppUser> _signInManager { get; }
        RoleManager<IdentityRole>? _roleManager { get; }

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser AppUser = await _userManager.FindByNameAsync(registerVM.Username);

            if (AppUser != null)
            {
                ModelState.AddModelError("Username", "Bu istifadeci artiq movcuddur.");
                return View();
            }

            AppUser appUser = new AppUser
            {
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                UserName = registerVM.Username,
                Email = registerVM.email,
            };

            IdentityResult result = await _userManager.CreateAsync(appUser, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(appUser  , "Memember");
            await _signInManager.SignInAsync(appUser, true);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginVM loginVM, string? ReturnUrl)
        {
            if (!ModelState.IsValid) return View();
            AppUser appUser = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            if (appUser is null)
            {
                appUser = await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);
                

                if (appUser is null)
                {
                    ModelState.AddModelError("", "Bele bir hesab yoxdur.");
                    return View();
                }
            }

            var result = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, loginVM.IsParsistance, true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "hesab Movcud deyil.");
                return View();
            }
            if (ReturnUrl==null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(ReturnUrl);
            }


            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        //public async Task<IActionResult> Test()
        //{
        ////    var user = await _userManager.FindByEmailAsync("ali44");
        ////    await _userManager.AddToRoleAsync(user, "Member");
        ////    user = await _userManager.FindByEmailAsync("ruslan15");
        ////    await _userManager.AddToRoleAsync(user, "Member");
        ////    return View();
        //    AppUser appUser = new AppUser
        //    {
        //        FirstName = "admin",
        //        LastName = "admin",
        //        UserName = "admin",
        //        Email = "tu6hwwz7l@code.edu.az"
        //    };
        //    await _userManager.CreateAsync(appUser, "Admin123");
        //    await _userManager.AddToRoleAsync(appUser, "Admin");
        //    return View();
        //}
        //public async Task<IActionResult> AddRoles()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Moderator" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });

        //    return View();
        //} // evvelce bu hisseni aciq ran edirik sonra comente aliriq//

    }
}
