using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Domain.Constants;

namespace Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IUserService _userService;

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        ILogger<AccountController> logger,
        IUserService userService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _userService = userService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");
        if (remoteError != null)
        {
            _logger.LogError($"Error from external provider: {remoteError}");
            return RedirectToAction("Error", "Home");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            _logger.LogError("Error loading external login information.");
            return RedirectToAction("Error", "Home");
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Email claim not found in external login info.");
            return RedirectToAction("Error", "Home");
        }

        _logger.LogInformation($"Attempting login for email: {email}");

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);

            var userResult = await _userService.CreateUser(email, Roles.Client, firstName, lastName);

            if (!userResult.IsSuccess)
            {
                _logger.LogWarning($"Error to create user for email: {email}");
                return RedirectToAction("UserNotFound");
            }

            user = await _userManager.FindByEmailAsync(email);
        }

        return await HandleExistingUser(user, info, returnUrl);
    }

    private async Task<IActionResult> HandleExistingUser(IdentityUser user, ExternalLoginInfo info, string returnUrl)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (info == null) throw new ArgumentNullException(nameof(info));

        _logger.LogInformation($"User found: {user.Id}");

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

        if (!signInResult.Succeeded)
        {
            var addLoginResult = await AddExternalLoginIfNeeded(user, info);
            if (!addLoginResult.Succeeded)
            {
                return RedirectToAction("Error", "Home");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        _logger.LogInformation($"User {user.Email} logged in using {info.LoginProvider}.");
        return LocalRedirect(returnUrl ?? Url.Content("~/"));
    }

    private async Task<IdentityResult> AddExternalLoginIfNeeded(IdentityUser user, ExternalLoginInfo info)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (info == null) throw new ArgumentNullException(nameof(info));

        var existingLogins = await _userManager.GetLoginsAsync(user);
        var existingLogin = existingLogins.FirstOrDefault(l => l.LoginProvider == info.LoginProvider && l.ProviderKey == info.ProviderKey);

        if (existingLogin != null)
        {
            _logger.LogInformation($"External login already exists for user {user.Email}.");
            return IdentityResult.Success;
        }

        var addLoginResult = await _userManager.AddLoginAsync(user, info);
        if (addLoginResult.Succeeded)
        {
            _logger.LogInformation($"External login added for user {user.Email}.");
        }
        else
        {
            _logger.LogWarning($"Failed to add external login for user {user.Id}. Errors: {string.Join(", ", addLoginResult.Errors.Select(e => e.Description))}");
        }

        return addLoginResult;
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult UserNotFound()
    {
        return View();
    }
}
