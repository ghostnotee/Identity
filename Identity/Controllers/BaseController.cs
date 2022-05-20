using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

public class BaseController : Controller
{
    protected readonly ILogger<HomeController> _logger;
    protected readonly UserManager<AppUser> _userManager;
    protected readonly SignInManager<AppUser> _signInManager;
    protected AppUser CurrentUser => _userManager.FindByNameAsync(User.Identity.Name).Result;

    public BaseController(ILogger<HomeController> logger, UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public void AddModelError(IdentityResult identityResult)
    {
        foreach (var error in identityResult.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
    }
}