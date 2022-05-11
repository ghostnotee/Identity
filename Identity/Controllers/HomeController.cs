using Identity.Models;
using Microsoft.AspNetCore.Mvc;
using Identity.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Identity.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;

    public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult LogIn()
    {
        return View();
    }

    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(UserViewModel userViewModel)
    {
        if (ModelState.IsValid)
        {
            AppUser user = new();
            user.UserName = userViewModel.UserName;
            user.Email = userViewModel.Email;
            user.PhoneNumber = userViewModel.PhoneNumber;

            IdentityResult result = await _userManager.CreateAsync(user, userViewModel.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("LogIn");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
        }

        return View(userViewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}