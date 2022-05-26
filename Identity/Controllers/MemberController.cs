using Identity.Enums;
using Identity.Models;
using Identity.Models.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Identity.Controllers;

[Authorize]
public class MemberController : BaseController
{
    public MemberController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager) : base(userManager, signInManager)
    {
    }

    public IActionResult Index()
    {
        var user = CurrentUser;
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
            var user = CurrentUser;

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
                    AddModelError(result);
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
        var user = CurrentUser;
        var userViewModel = user.Adapt<UserViewModel>();
        ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));

        return View(userViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UserEdit(UserViewModel userViewModel, IFormFile userPicture)
    {
        ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
        ModelState.Remove("Password");
        //if (ModelState.IsValid)
        // {
        var user = CurrentUser;
        if (userPicture is { Length: > 0 })
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserPicture", fileName);
            await using var stream = new FileStream(path, FileMode.Create);
            await userPicture.CopyToAsync(stream);
            user.Picture = "UserPicture/" + fileName;
        }

        user.UserName = userViewModel.UserName;
        user.Email = userViewModel.Email;
        user.PhoneNumber = userViewModel.PhoneNumber;
        user.City = userViewModel.City;
        user.BirthDay = userViewModel.BirthDay;
        user.Gender = (ushort)userViewModel.Gender;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            await _userManager.UpdateSecurityStampAsync(user);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, true);
            ViewBag.success = "true";
        }
        else
        {
            AddModelError(result);
        }
        //}

        return View(userViewModel);
    }

    public void Logout()
    {
        _signInManager.SignOutAsync();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    [Authorize(Roles = "Editor,Admin")]
    public IActionResult Editor()
    {
        return View();
    }

    [Authorize(Roles = "Manager,Admin")]
    public IActionResult Manager()
    {
        return View();
    }

    [Authorize(Policy = "KutahyaPolicy")]
    public IActionResult KutahyaPage()
    {
        return View();
    }
}