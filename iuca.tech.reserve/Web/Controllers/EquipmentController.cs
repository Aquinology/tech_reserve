using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Common;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace Web.Controllers;

public class EquipmentController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEquipmentService _equipmentService;
    private readonly IFileService _fileService;

    public EquipmentController(IWebHostEnvironment webHostEnvironment,
        IEquipmentService equipmentService,
        IFileService fileService)
    {
        _webHostEnvironment = webHostEnvironment;
        _equipmentService = equipmentService;
        _fileService = fileService;
    }

    public async Task<IActionResult> Index()
    {
        var equipments = await _equipmentService.GetAllEquipments();

        ViewBag.EquipmentTypes = new SelectList(Enum.GetValues(typeof(EquipmentType)));

        return View(equipments.Data);
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorException { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
