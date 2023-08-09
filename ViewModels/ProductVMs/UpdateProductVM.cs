using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.ProductVMs;

public record UpdateProductVM
{
    public int Id { get; set; }
    [Required, MaxLength(64)]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required, Range(0, Double.MaxValue)]
    public decimal Price { get; set; }
    [Required, Range(0, 100)]
    public byte Discount { get; set; }
    [Required, Range(0, Int32.MaxValue)]
    public int StockCount { get; set; }
    [Required, Range(0, 5)]
    public byte Rating { get; set; }
    public IFormFile? MainImage { get; set; }
    public IFormFile? HoverImage { get; set; }
    public ICollection<IFormFile>? ProductImages { get; set; }
    [Required]
    public List<int> CategoryIds { get; set; }
}

