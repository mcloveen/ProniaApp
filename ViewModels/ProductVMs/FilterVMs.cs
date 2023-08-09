namespace Pronia.ViewModels.ProductVMs;

public record FilterVM
{
    public string Search { get; set; }
    public int CategoryId { get; set; }
    public int MinPrice { get; set; }
    public int MaxPrice { get; set; }
}

