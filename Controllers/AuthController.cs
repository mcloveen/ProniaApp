using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Models;
using Pronia.ViewModels.AuthVMs;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Pronia.Controllers;
public class AuthController : Controller
{
    readonly UserManager<AppUser> _userManager;
    readonly SignInManager<AppUser> _signInManager;
    readonly RoleManager<IdentityRole> _roleManager;

    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (!ModelState.IsValid) return View();
        AppUser user = new AppUser
        {
            Fullname = vm.Name + " " + vm.Surname,
            Email = vm.Email,
            UserName = vm.Username
        };
        var result = await _userManager.CreateAsync(user, vm.Password);
        if (!result.Succeeded)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
            return View();
        }
        var res = await _userManager.AddToRoleAsync(user, "Member");
        if (!res.Succeeded)
        {
            foreach (var item in res.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
            return View();
        }
        return RedirectToAction(nameof(Login));
    }
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(string? ReturnUrl, LoginVM vm)
    {
        if (!ModelState.IsValid) return View();
        //if(!await _userManager.Users.AnyAsync(u => u.Email == vm.UsernameOrEmail || u.UserName == vm.UsernameOrEmail))
        //{
        //    ModelState.AddModelError("", "Username, email or password is wrong");
        //}

        var user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);
        if (user == null)
        {
            user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "Username, email or password is wrong");
                return View();
            }
        }
        var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);
        if (result.IsLockedOut)
        {
            ModelState.AddModelError("", "Wait until " + user.LockoutEnd.Value.AddHours(4).ToString("HH:mm:ss"));
            return View();
        }
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Username, email or password is wrong");
            return View();
        }
        if (ReturnUrl == null)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            return Redirect(ReturnUrl);
        }
    }
    public async Task<IActionResult> SignOut()
    {
        await _signInManager.SignOutAsync();
        return View(nameof(Login));
    }

    //public async Task CreateRoles()
    //{
    //    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
    //    await _roleManager.CreateAsync(new IdentityRole { Name = "Editor" });
    //    await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });
    //}
}