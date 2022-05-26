using Identity.Models;
using Microsoft.AspNetCore.Mvc;
using Identity.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Identity.Controllers;

public class HomeController : BaseController
{
    public HomeController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager) : base(userManager, signInManager)
    {
    }

    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Member");
        }

        return View();
    }

    public IActionResult LogIn(string ReturnUrl)
    {
        TempData["ReturnUrl"] = ReturnUrl;

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
                if (await _userManager.IsLockedOutAsync(user))
                {
                    ModelState.AddModelError("", "Hesabınız kilitlidir, Daha sonra tekrar deneyin");
                    return View(loginViewModel);
                }

                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password,
                    loginViewModel.RememberMe, false);

                if (result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user);

                    if (TempData["ReturnUrl"] != null)
                        return Redirect(TempData["ReturnUrl"].ToString());

                    return RedirectToAction("Index", "Member");
                }
                else
                {
                    await _userManager.AccessFailedAsync(user);

                    var failCount = await _userManager.GetAccessFailedCountAsync(user);
                    if (failCount == 3)
                    {
                        await _userManager.SetLockoutEndDateAsync(user,
                            new DateTimeOffset(DateTime.Now.AddMinutes(20)));
                        ModelState.AddModelError("", "Hesabınız kilitlenmiştir");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Geçersiz email adresi veya şifresi");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Geçersiz email adresi veya şifresi");
            }
        }

        return View(loginViewModel);
    }

    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(UserViewModel userViewModel)
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
            AddModelError(result);
        }


        return View(userViewModel);
    }

    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ResetPassword(PasswordResetViewModel passwordResetViewModel)
    {
        var user = _userManager.FindByEmailAsync(passwordResetViewModel.Email).Result;
        if (user is not null)
        {
            string passwordResetToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;
            var passwordResetLink = Url.Action("ResetPasswordConfirm", "Home", new
            {
                userId = user.Id,
                token = passwordResetToken
            }, HttpContext.Request.Scheme);
            Helper.PasswordReset.PasswordResetSendEmail(passwordResetLink);
            ViewBag.status = "successfull";
        }
        else
        {
            ModelState.AddModelError("", "Sistemde kayıtlı email adresi bulunamadı");
        }


        return View(passwordResetViewModel);
    }

    public IActionResult ResetPasswordConfirm(string userId, string token)
    {
        TempData["userId"] = userId;
        TempData["token"] = token;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPasswordConfirm(
        [Bind("PasswordNew")] PasswordResetViewModel passwordResetViewModel)
    {
        var token = TempData["token"]?.ToString();
        var userId = TempData["userId"]?.ToString();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is not null)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, passwordResetViewModel.PasswordNew);

            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                TempData["passwordResetInfo"] = "Şifre yenilenmiştir.";
                ViewBag.status = "success";
            }
            else
            {
                AddModelError(result);
            }
        }
        else
        {
            ModelState.AddModelError("", "Kullanıcı bulunamadı.");
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}