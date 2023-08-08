using System;
namespace Pronia.Extensions;

public static class FileExtension
{
    public static bool IsSizeValid(this IFormFile file, int mb)
      => file.Length <= mb * 1024 * 1024;

    public static bool IsTypeValid(this IFormFile file, string contentType)
        => file.ContentType.StartsWith(contentType);
}

