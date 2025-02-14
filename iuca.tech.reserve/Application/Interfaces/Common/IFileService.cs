using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Common;

public interface IFileService
{
    Task<string> SaveFile(IFormFile file);
    void DeleteFile(string relativePath);
}
