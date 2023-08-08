using System;
namespace Pronia.ExtensionServices.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file, string path, string contentType = "image", int mb = 2);
        Task SaveAsync(IFormFile file, string path);
        void Delete(string path);
    }
}

