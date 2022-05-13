using Identity.Models;
using Microsoft.AspNetCore.Mvc;
using Identity.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Identity.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult LogIn()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LogIn(LoginViewModel loginViewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (user != null)
            {
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Member");
                }
            }
            else
            {
                ModelState.AddModelError("","Geçersiz email adresi veya şifresi");
            }
            
        }

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