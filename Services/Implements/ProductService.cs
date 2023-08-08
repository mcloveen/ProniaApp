using Microsoft.EntityFrameworkCore;
using Pronia.DataAccess;
using Pronia.ExtensionServices.Interfaces;
using Pronia.Models;
using Pronia.ViewModels.ProductVMs;
using Pronia.Services.Interfaces;

namespace Pronia.Services.Implements;

public class ProductService : IProductService
{
    private readonly ProniaDbContext _context;
    readonly IFileService _fileService;
    readonly ICategoryService _categoryService;

    IQueryable<Product> IProductService.GetTable { get => _context.Set<Product>(); }

    public ProductService(ProniaDbContext context,
        IFileService fileService, ICategoryService categoryService)
    {
        _context = context;
        _fileService = fileService;
        _categoryService = categoryService;
    }

    public async Task Create(CreateProductVM productVM)
    {
        if (productVM.CategoryIds.Count > 4)
            throw new Exception();
        if (!await _categoryService.IsAllExist(productVM.CategoryIds))
            throw new ArgumentException();
        List<ProductCategory> productCategories = new List<ProductCategory>();
        foreach (var id in productVM.CategoryIds)
        {
            productCategories.Add(new ProductCategory
            {
                CategoryId = id
            });
        }
        Product entity = new Product()
        {
            Name = productVM.Name,
            Description = productVM.Description,
            Discount = productVM.Discount,
            Price = productVM.Price,
            Rating = productVM.Rating,
            StockCount = productVM.StockCount,
            MainImage = await _fileService.UploadAsync(productVM.MainImageFile, Path.Combine(
                "assets", "imgs", "products")),
            ProductCategories = productCategories
        };
        if (productVM.ImageFiles != null)
        {
            List<ProductImage> imgs = new();
            foreach (var item in productVM.ImageFiles)
            {
                string fileName = await _fileService.UploadAsync(item, Path.Combine(
                "assets", "imgs", "products"));
                imgs.Add(new ProductImage
                {
                    Name = fileName
                });
            }
            entity.ProductImages = imgs;
        }
        if (productVM.HoverImageFile != null)
            entity.HoverImage = await _fileService.UploadAsync(productVM.HoverImageFile, Path.Combine(
                "assets", "imgs", "products"));
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int? id)
    {
        var entity = await GetById(id, true);
        _context.Remove(entity);
        _fileService.Delete(entity.MainImage);
        if (entity.HoverImage != null)
        {
            _fileService.Delete(entity.HoverImage);
        }
        if (entity.ProductImages != null)
        {
            foreach (var item in entity.ProductImages)
            {
                _fileService.Delete(item.Name);
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<Product>> GetAll(bool takeAll)
    {
        if (takeAll)
        {
            return await _context.Products.ToListAsync();
        }
        return await _context.Products.Where(p => p.IsDeleted == false).ToListAsync();
    }

    public async Task<Product> GetById(int? id, bool takeAll = false)
    {
        if (id == null || id < 1) throw new ArgumentException();
        Product? entity;
        if (takeAll)
        {
            entity = await _context.Products.FindAsync(id);
        }
        else
        {
            entity = await _context.Products.SingleOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
        }
        if (entity == null) throw new ArgumentNullException();
        return entity;
    }


    public async Task SoftDelete(int? id)
    {
        var entity = await GetById(id);
        entity.IsDeleted = !entity.IsDeleted;
        await _context.SaveChangesAsync();
    }

    public async Task Update(int? id, UpdateProductVM vm)
    {
        if (vm.CategoryIds.Count > 4)
            throw new Exception();
        if (!await _categoryService.IsAllExist(vm.CategoryIds))
            throw new ArgumentException();
        List<ProductCategory> productCategories = new List<ProductCategory>();
        foreach (var catid in vm.CategoryIds)
        {
            productCategories.Add(new ProductCategory
            {
                CategoryId = catid
            });
        }
        var entity = await _context.Products.Include(p => p.ProductCategories).
            SingleOrDefaultAsync(p => p.Id == id);
        if (entity.ProductCategories != null)
        {
            entity.ProductCategories.Clear();
        }
        entity.Name = vm.Name;
        entity.Description = vm.Description;
        entity.Price = vm.Price;
        entity.Discount = vm.Discount;
        entity.StockCount = vm.StockCount;
        entity.Rating = vm.Rating;
        entity.ProductCategories = productCategories;
        if (vm.MainImage != null)
        {
            _fileService.Delete(entity.MainImage);
            entity.MainImage = await _fileService.UploadAsync(vm.MainImage,
                Path.Combine("assets", "imgs", "products"));
        }
        if (vm.HoverImage != null)
        {
            if (entity.HoverImage != null)
            {
                _fileService.Delete(entity.HoverImage);
            }
            entity.HoverImage = await _fileService.UploadAsync(vm.HoverImage,
                Path.Combine("assets", "imgs", "products"));
        }
        if (vm.ProductImages != null)
        {
            if (entity.ProductImages == null) entity.ProductImages = new List<ProductImage>();
            foreach (var item in vm.ProductImages)
            {
                ProductImage productImage = new ProductImage
                {
                    Name = await _fileService.UploadAsync(item,
                    Path.Combine("assets", "imgs", "products"))
                };
                entity.ProductImages.Add(productImage);
            }
        }
        await _context.SaveChangesAsync();

    }

    public async Task DeleteImage(int? id)
    {
        if (id == null || id <= 0) throw new ArgumentNullException();
        var entity = await _context.ProductImages.FindAsync(id);
        if (entity == null) throw new NullReferenceException();
        _fileService.Delete(entity.Name);
        _context.ProductImages.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

