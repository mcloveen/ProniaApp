using System;
using Pronia.Models;

namespace Pronia.ViewModels.HomeVMs;

public class HomeVm
{
    public ICollection<Slider> Sliders { get; set; }
    public ICollection<Product> Products { get; set; }
}

