using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class ClientsController : Controller
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [Authorize(Roles = Roles.Client)]
    [HttpPost]
    public async Task<IActionResult> UpdatePhoneNumber(string applicationUserId, string phoneNumber)
    {
        if (!ModelState.IsValid)
        {
            var errorMessage = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
            .FirstOrDefault();

            return Json(new { isSuccess = false, message = errorMessage });
        }

        var result = await _clientService.UpdateClientPhoneNumber(applicationUserId, phoneNumber);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }
}
