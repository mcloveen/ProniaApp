using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
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
        return RedirectToAction(nameof(Login));
    }
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm)
    {
        if (!ModelState.IsValid) return View();
     

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
        if (user.LockoutEnd != null)
        {
            ModelState.AddModelError("", "Wait until " + user.LockoutEnd.Value.AddHours(4).ToString("HH:mm:ss"));
            return View();
        }
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Username, email or password is wrong");
            return View();
        }
        return RedirectToAction("Index", "Home");
    }
    public async Task<IActionResult> SignOut()
    {
        await _signInManager.SignOutAsync();
        return View(nameof(Login));
    }
}

