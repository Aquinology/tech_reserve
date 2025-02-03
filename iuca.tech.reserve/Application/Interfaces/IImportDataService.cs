using Application.DTOs.Common;

namespace Application.Interfaces;

public interface IImportDataService
{
    Task<Result> ImportClients(string connection);
}
