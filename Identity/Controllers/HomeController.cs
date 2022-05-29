using System.Security.Claims;
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

                if (_userManager.IsEmailConfirmedAsync(user).Result is false)
                {
                    ModelState.AddModelError("", "Email adresiniz doğrulanmamıştır. Emailinizi kontrol ediniz.");
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
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("ConfirmEmail", "Home", new
            {
                userId = user.Id,
                token = confirmationToken
            }, protocol: HttpContext.Request.Scheme);

            Helper.EmailConfirmation.SendEmail(link, user.Email);

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
            Helper.PasswordReset.PasswordResetSendEmail(passwordResetLink, passwordResetViewModel.Email);
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

    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            ViewBag.status = "Email adresiniz onaylanmıştır.";
        }
        else
        {
            ViewBag.status = "Hata meydana geldi.";
        }

        return View();
    }

    public IActionResult FacebookLogin(string returnUrl)
    {
        var redirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = returnUrl });

        var property = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
        return new ChallengeResult("Facebook", property);
    }

    public IActionResult GoogleLogin(string returnUrl)
    {
        var redirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = returnUrl });

        var property = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        return new ChallengeResult("Google", property);
    }


    public IActionResult MicrosoftLogin(string returnUrl)
    {
        var redirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = returnUrl });

        var property = _signInManager.ConfigureExternalAuthenticationProperties("Microsoft", redirectUrl);
        return new ChallengeResult("Microsoft", property);
    }


    public async Task<IActionResult> ExternalResponse(string returnUrl = "/")
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            return RedirectToAction("Login");
        }
        else
        {
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }
            else
            {
                var user = new AppUser();
                var externalUSerId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                user.Email = info.Principal.FindFirst(ClaimTypes.Email).Value;
                user.Id = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
                {
                    var userName = info.Principal.FindFirst(ClaimTypes.Name).Value;
                    userName = userName.Replace(' ', '-').ToLower() + externalUSerId.Substring(0, 5);
                    user.UserName = userName;
                }
                else
                {
                    user.UserName = info.Principal.FindFirst(ClaimTypes.Email).Value;
                }

                var userExist = await _userManager.FindByEmailAsync(ClaimTypes.Email);
                if (userExist is null)
                {
                    var createResult = await _userManager.CreateAsync(user);

                    if (createResult.Succeeded)
                    {
                        var loginResult = await _userManager.AddLoginAsync(user, info);
                        if (loginResult.Succeeded)
                        {
                            await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            AddModelError(loginResult);
                        }
                    }
                    else
                    {
                        AddModelError(createResult);
                    }
                }
                else
                {
                    var loginResult = await _userManager.AddLoginAsync(userExist, info);
                    await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                    return Redirect(returnUrl);
                }
            }
        }

        var errors = ModelState.Values.SelectMany(x => x.Errors).Select(y => y.ErrorMessage).ToList();
        return RedirectToAction("Error", errors);
    }

    public ActionResult Error()
    {
        return View();
    }
}