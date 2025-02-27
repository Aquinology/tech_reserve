using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace Web.Controllers;

public class EquipmentController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEquipmentService _equipmentService;
    private readonly IRequestService _requestService;

    public EquipmentController(UserManager<IdentityUser> userManager,
        IEquipmentService equipmentService,
        IRequestService requestService)
    {
        _userManager = userManager;
        _equipmentService = equipmentService;
        _requestService = requestService;
    }

    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return View();

        var equipments = await _equipmentService.GetAllEquipments();
        var request = await _requestService.GetActualRequest(currentUser.Id);

        ViewBag.EquipmentTypes = new SelectList(Enum.GetValues(typeof(EquipmentType)));

        return View((equipments.Data, request.Data));
    }

    [HttpPost]
    public async Task<IActionResult> Create(EquipmentDTO equipment)
    {
        if (!ModelState.IsValid)
        {
            var errorMessage = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault();

            return Json(new { isSuccess = false, message = errorMessage });
        }

        var result = await _equipmentService.CreateEquipment(equipment);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int equipmentId, EquipmentDTO equipment)
    {
        if (!ModelState.IsValid)
        {
            var errorMessage = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault();

            return Json(new { isSuccess = false, message = errorMessage });
        }

        var result = await _equipmentService.EditEquipment(equipmentId, equipment);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int equipmentId)
    {
        var result = await _equipmentService.DeleteEquipment(equipmentId);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorException { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
