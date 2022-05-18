using Identity.Models;
using Identity.Models.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[Authorize]
public class MemberController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public MemberController(ILogger<HomeController> logger, UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Index()
    {
        var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
        UserViewModel userViewModel = user.Adapt<UserViewModel>();


        return View(userViewModel);
    }

    public IActionResult PasswordChange()
    {
        return View();
    }

    [HttpPost]
    public IActionResult PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
    {
        if (ModelState.IsValid)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            bool exist = _userManager.CheckPasswordAsync(user, passwordChangeViewModel.PasswordOld).Result;
            if (exist)
            {
                IdentityResult result = _userManager.ChangePasswordAsync(user, passwordChangeViewModel.PasswordOld,
                    passwordChangeViewModel.PasswordNew).Result;
                if (result.Succeeded)
                {
                    _userManager.UpdateSecurityStampAsync(user);
                    
                    _signInManager.SignOutAsync();
                    _signInManager.PasswordSignInAsync(user, passwordChangeViewModel.PasswordNew, true, false);
                    
                    ViewBag.success = true;
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Sifreniz yanlış.");
            }
        }

        return View();
    }


    public IActionResult UserEdit()
    {
        return View();
    }


    public IActionResult Logout()
    {
        return View();
    }
}