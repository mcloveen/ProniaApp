using Pronia.ExtensionServices.Implements;
using Pronia.ExtensionServices.Interfaces;
using Pronia.Services.Implements;
using Pronia.Services.Interfaces;

namespace Pronia.Services;

public static class ServiceRegistration
{
    public static void AddService(this IServiceCollection services)
    {
        services.AddScoped<ISliderService, SliderService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<LayoutService>();
    }
}

