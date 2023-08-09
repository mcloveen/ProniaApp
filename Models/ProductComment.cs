using System;
namespace Pronia.Models;

public class ProductComment : BaseEntity
{
    public string UserId { get; set; }
    public int ProductsId { get; set; }
    public string Comment { get; set; }
    public DateTime PostedTime { get; set; }
    public Product? Product { get; set; }
    public AppUser? AppUser { get; set; }
    public bool IsDeleted { get; set; }
    public int? ParentId { get; set; }
    public ProductComment? Parent { get; set; }
    public ICollection<ProductComment>? Children { get; set; }
}

