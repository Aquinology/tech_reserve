using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Authorize(Roles = Roles.Administrator)]
public class ImportDataController : Controller
{
    private readonly string _iucaDbConnection;

    private readonly IImportDataService _importDataService;

    public ImportDataController(IConfiguration configuration,
        IImportDataService importDataService)
    {
        _iucaDbConnection = configuration.GetConnectionString("IucaDbConnection")!;

        _importDataService = importDataService;
    }

    [HttpPost]
    public async Task<IActionResult> ImportClients()
    {
        var result = await _importDataService.ImportClients(_iucaDbConnection);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }
}
