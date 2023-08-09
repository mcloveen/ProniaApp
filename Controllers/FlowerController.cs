using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DataAccess;
using Pronia.Models;
using Pronia.Services.Interfaces;
using Pronia.ViewModels.ProductVMs;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Pronia.Controllers;
public class FlowerController : Controller
{
    readonly IProductService _service;
    readonly ProniaDbContext _context;
    readonly IProductService _prodService;
    readonly UserManager<AppUser> _userManager;
    public FlowerController(IProductService service, ProniaDbContext context,
        IProductService productService, UserManager<AppUser> userManager)
    {
        _service = service;
        _context = context;
        _prodService = productService;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        IQueryable<Product> products = _service.GetTable;
        GetShopVM vm = new GetShopVM
        {
            Products = await products.Include(p => p.ProductImages).Take(4).ToListAsync(),
            ProductCount = await products.CountAsync(),
            Categories = _context.Categories.Include(p => p.ProductCategories),
            CategoryCount = await _context.ProductCategories.CountAsync()

        };
        return View(vm);
    }
    public async Task<IActionResult> Detail(int? id)
    {
        if (id == null || id <= 0) return BadRequest();

        //var entity = await _service.GetById(id);
        var entity = await _service.GetTable.Include(p => p.ProductImages).
            Include(p => p.ProductComments).ThenInclude(pc => pc.AppUser).
            SingleOrDefaultAsync(p => p.Id == id
        && p.IsDeleted == false);
        if (entity == null) return NotFound();
        return View(entity);
    }
    [HttpPost]
    public async Task<IActionResult> Filter(FilterVM vm)
    {
        if (vm.MinPrice > vm.MaxPrice) return BadRequest();
        if (String.IsNullOrWhiteSpace(vm.Search))
        {
            vm.Search = "";
        }
        var model = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category);
        var result = model.Where(p => p.Name.Contains(vm.Search));
        if (vm.CategoryId > 0)
        {
            result = result.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == vm.CategoryId));
        }
        if (vm.MaxPrice != 0)
        {
            result = result.Where(p => p.Price <= vm.MaxPrice && p.Price >= vm.MinPrice);
        }
        return PartialView("_ProductsFilterPartial", result);
    }
    public async Task<IActionResult> Pagination(int skip = 4, int take = 4)
    {
        return PartialView("_ProductsFilterPartial",
            await _prodService.GetTable.Skip(skip).Take(take).ToListAsync());
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostComment(AddCommentToProductVM vm)
    {
        if (vm.ProductId <= 0)
        {
            return BadRequest();
        }
        if (!await _context.Products.AnyAsync(p => p.Id == vm.ProductId))
        {
            return NotFound();
        }
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        if (user == null)
        {
            return NotFound();
        }
        await _context.ProductComments.AddAsync(new ProductComment
        {
            ProductsId = vm.ProductId,
            AppUser = user,
            UserId = user.Id,
            Comment = vm.Comment,
            PostedTime = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Detail), new { id = vm.ProductId });
    }
}
