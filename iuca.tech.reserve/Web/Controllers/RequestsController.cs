using Application.Interfaces;
using Domain.Constants;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Controllers;

public class RequestsController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEquipmentRequestService _equipmentRequestService;
    private readonly IRequestService _requestService;

    public RequestsController(UserManager<IdentityUser> userManager,
        IEquipmentRequestService equipmentRequestService,
        IRequestService requestService)
    {
        _userManager = userManager;
        _equipmentRequestService = equipmentRequestService;
        _requestService = requestService;
    }

    [Authorize(Roles = Roles.Administrator)]
    public async Task<IActionResult> Index(RequestStatus status)
    {
        var requestsResult = await _requestService.GetRequests(status != RequestStatus.None ? status : null);

        ViewBag.RequestStatuses = new SelectList(Enum.GetValues(typeof(RequestStatus)), status);

        return View(requestsResult.Data);
    }

    [Authorize(Roles = Roles.Client)]
    [HttpPost]
    public async Task<IActionResult> AddEquipmentToRequest(int equipmentId)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Json(new { isSuccess = false, message = $"User not found." });

        var result = await _equipmentRequestService.AddEquipmentToRequest(currentUser.Id, equipmentId);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }

    [Authorize(Roles = Roles.Client)]
    [HttpPost]
    public async Task<IActionResult> RemoveEquipmentFromRequest(int equipmentId)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Json(new { isSuccess = false, message = $"User not found." });

        var result = await _equipmentRequestService.RemoveEquipmentFromRequest(currentUser.Id, equipmentId);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }

    [Authorize(Roles = Roles.Client)]
    [HttpPost]
    public async Task<IActionResult> CancelRequest(int requestId)
    {
        var result = await _requestService.SetRequestStatus(requestId, RequestStatus.Canceled);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }

    [Authorize(Roles = Roles.Administrator)]
    [HttpPost]
    public async Task<IActionResult> ApproveRequest(int requestId)
    {
        var result = await _requestService.SetRequestStatus(requestId, RequestStatus.Issued);

        if (result.IsSuccess)
        {
            result = await _requestService.SetIssuedDate(requestId, DateTime.UtcNow);
        }

        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }

    [Authorize(Roles = Roles.Administrator)]
    [HttpPost]
    public async Task<IActionResult> RejectRequest(int requestId)
    {
        var result = await _requestService.SetRequestStatus(requestId, RequestStatus.Rejected);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }

    [Authorize(Roles = Roles.Administrator)]
    [HttpPost]
    public async Task<IActionResult> ReturnEquipment(int requestId)
    {
        var result = await _requestService.SetRequestStatus(requestId, RequestStatus.Returned);

        if (result.IsSuccess)
        {
            result = await _requestService.SetReturnedDate(requestId, DateTime.UtcNow);
        }

        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }
}
