using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronia.Services.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Pronia.Areas.Manage.Controllers;

[Area("Manage")]
[Authorize(Roles = "Admin ,Editor")]
public class CategoryController : Controller
{
    readonly ICategoryService _service;
    public CategoryController(ICategoryService service)
    {
        _service = service;
    }
    public async Task<IActionResult> Index()
    {
        return View(await _service.GetAll());
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(string name)
    {
        if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name)) return BadRequest();
        await _service.Create(name);
        return View();
    }
}