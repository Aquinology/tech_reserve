using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Authorize]
public class RequestController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEquipmentRequestService _equipmentRequestService;

    public RequestController(UserManager<IdentityUser> userManager,
        IEquipmentRequestService equipmentRequestService)
    {
        _userManager = userManager;
        _equipmentRequestService = equipmentRequestService;
    }

    [HttpPost]
    public async Task<IActionResult> AddEquipmentToRequest(int equipmentId)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Json(new { isSuccess = false, message = $"User not found." });

        var result = await _equipmentRequestService.AddEquipmentToRequest(currentUser.Id, equipmentId);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveEquipmentFromRequest(int equipmentId)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Json(new { isSuccess = false, message = $"User not found." });

        var result = await _equipmentRequestService.RemoveEquipmentFromRequest(currentUser.Id, equipmentId);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }
}
