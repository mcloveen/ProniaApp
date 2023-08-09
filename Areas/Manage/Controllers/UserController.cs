using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pronia.Models;
using Pronia.ViewModels.UserVMs;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Pronia.Areas.Manage.Controllers;

[Area("Manage")]
[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    readonly UserManager<AppUser> _userManager;
    readonly RoleManager<IdentityRole> _roleManager;
    public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        List<UserAndRolesVM> vm = new List<UserAndRolesVM>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            vm.Add(new UserAndRolesVM
            {
                FullName = user.Fullname,
                Name = user.UserName,
                Role = roles.FirstOrDefault()
            });
        }
        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        return View(vm);
    }
    [HttpPost]
    public async Task<IActionResult> ChangeRole(string username, string role)
    {
        var currentUser = await _userManager.FindByNameAsync(User.Identity?.Name);
        var user = await _userManager.FindByNameAsync(username);
        if (currentUser == null || user == null) return BadRequest();
        var UserRoles = await _userManager.GetRolesAsync(user);
        if (UserRoles.FirstOrDefault() == "Admin")
        {
            return BadRequest();
        }
        else
        {
            await _userManager.RemoveFromRolesAsync(user, UserRoles);
            await _userManager.AddToRoleAsync(user, role);
        }
        return Ok();
    }
}

