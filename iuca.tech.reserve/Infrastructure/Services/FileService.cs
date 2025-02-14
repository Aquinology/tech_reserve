using Application.Interfaces.Common;
using Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<EquipmentService> _logger;

    public FileService(IWebHostEnvironment webHostEnvironment,
        ILogger<EquipmentService> logger)
    {
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    public async Task<string> SaveFile(IFormFile file)
    {
        try
        {
            using (var fileStream = file.OpenReadStream())
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
                {
                    await fileStream.CopyToAsync(fileStreamOutput);
                }

                return Path.Combine("uploads", uniqueFileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file: {Message}", ex.Message);
            throw;
        }
    }

    public void DeleteFile(string relativePath)
    {
        try
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {Message}", ex.Message);
            throw;
        }
    }
}
