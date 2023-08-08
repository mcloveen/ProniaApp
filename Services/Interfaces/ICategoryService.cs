using System;
using Pronia.Models;

namespace Pronia.Services.Interfaces;

public interface ICategoryService
{
    Task Create(string name);
    Task Update(int? id, string name);
    Task Delete(int? id);
    Task<ICollection<Category>> GetAll();
    Task<Category> GetById(int? id);
    IQueryable<Category> GetTable { get; }
    Task<bool> IsExist(int id);
    Task<bool> IsAllExist(List<int> ids);

}
