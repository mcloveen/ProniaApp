using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.SliderVMs;

public record UpdateSliderVM
{
    public int? Id { get; set; }
    [Required, MaxLength(50)]
    public string Title { get; set; }
    [Required, MaxLength(200)]
    public string Description { get; set; }
    [MaxLength(50)]
    public string? Offer { get; set; }
    [Required, MaxLength(50)]
    public string ButtonText { get; set; }
    public IFormFile ImageFile { get; set; }
}

