using Microsoft.EntityFrameworkCore;
using Pronia.DataAccess;
using Pronia.ExtensionServices.Interfaces;
using Pronia.Models;
using Pronia.ViewModels.SliderVMs;
using Pronia.Services.Interfaces;

namespace Pronia.Services.Implements;

public class SliderService : ISliderService
{
    private readonly ProniaDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IFileService _fileService;
    public SliderService(ProniaDbContext context, IWebHostEnvironment env, IFileService fileService = null)
    {
        _context = context;
        _env = env;
        _fileService = fileService;
    }
    public async Task Create(CreateSliderVM sliderVM)
    {
        await _context.Sliders.AddAsync(new Slider
        {
            ImageUrl = await _fileService.UploadAsync(sliderVM.ImageFile, Path.Combine("assets", "imgs"), "image", 2),
            Title = sliderVM.Title,
            Offer = sliderVM.Offer,
            ButtonText = sliderVM.ButtonText,
            Description = sliderVM.Description,
        });
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int? id)
    {
        var entity = await GetById(id);
        _context.Sliders.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<Slider>> GetAll()
    {
        return await _context.Sliders.ToListAsync();
    }

    public async Task<Slider> GetById(int? id)
    {
        if (id < 1 || id == null) throw new ArgumentException();
        var entity = await _context.Sliders.FindAsync(id);
        if (entity == null) throw new ArgumentNullException();
        return entity;
    }

    public async Task Update(UpdateSliderVM sliderVM)
    {
        var entity = await GetById(sliderVM.Id);
        entity.Title = sliderVM.Title;
        entity.Offer = sliderVM.Offer;
        entity.Description = sliderVM.Description;
        entity.ButtonText = sliderVM.ButtonText;
        //entity.ImageUrl = sliderVM.ImageUrl;
        await _context.SaveChangesAsync();

    }
}

