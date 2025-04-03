using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Controllers;

[Authorize(Roles = Roles.Administrator)]
public class UsersController : Controller
{
    private readonly ILogger<UsersController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserService _userService;
    private readonly IClientService _clientService;

    public UsersController(ILogger<UsersController> logger,
        UserManager<IdentityUser> userManager,
        IUserService userService,
        IClientService clientService)
    {
        _logger = logger;
        _userManager = userManager;
        _userService = userService;
        _clientService = clientService; 
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersWithRoles();

        ViewBag.Roles = new SelectList(new List<string>
        {
            Roles.Administrator,
            Roles.Client
        });

        return View(users.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string email, string firstName, string lastName,  string role)
    {
        var result = await _userService.CreateUser(email, role, firstName, lastName);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string userId)
    {
        var result = await _userService.DeleteUser(userId);
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> GenerateClientAccounts()
    {
        var result = await _userService.GenerateClientAccounts();
        return Json(new { isSuccess = result.IsSuccess, message = result.Message });
    }
}
