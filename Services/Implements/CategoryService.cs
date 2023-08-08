using Microsoft.EntityFrameworkCore;
using Pronia.DataAccess;
using Pronia.Models;
using Pronia.Services.Interfaces;

namespace Pronia.Services.Implements;

public class CategoryService : ICategoryService
{
    readonly ProniaDbContext _context;
    public CategoryService(ProniaDbContext context)
    {
        _context = context;
    }
    public IQueryable<Category> GetTable => _context.Set<Category>();
    public async Task Create(string name)
    {
        if (name == null) throw new ArgumentNullException();
        if (await _context.Categories.AnyAsync(c => c.Name == name))
            throw new Exception();
        await _context.Categories.AddAsync(new Category() { Name = name });
        await _context.SaveChangesAsync();
    }

    public Task Delete(int? id)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Category>> GetAll()
        => await _context.Categories.ToListAsync();

    public Task<Category> GetById(int? id)
    {
        throw new NotImplementedException();
    }

    public Task Update(int? id, string name)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsAllExist(List<int> ids)
    {
        foreach (var id in ids)
        {
            if (!await IsExist(id))
                return false;
        }
        return true;
    }

    public Task<bool> IsExist(int id)
        => _context.Categories.AnyAsync(c => c.Id == id);
}

