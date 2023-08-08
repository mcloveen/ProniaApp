using System;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Product : BaseEntity
    {
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
        public bool IsDeleted { get; set; }
        [Required]
        public string MainImage { get; set; }
        public string? HoverImage { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }
        public ICollection<ProductCategory>? ProductCategories { get; set; }

    }
}

