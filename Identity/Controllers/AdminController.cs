using Identity.Models;
using Identity.Models.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) : base(
            userManager, null, roleManager)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Roles()
        {
            return View(_roleManager.Roles.ToList());
        }


        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RoleCreate(RoleViewModel roleViewModel)
        {
            var role = new AppRole() { Name = roleViewModel.Name };
            var result = _roleManager.CreateAsync(role).Result;

            if (result.Succeeded)
            {
                return RedirectToAction("Roles");
            }
            else
            {
                AddModelError(result);
            }

            return View(roleViewModel);
        }

        public IActionResult Users()
        {
            return View(_userManager.Users);
        }

        public IActionResult RoleDelete(string id)
        {
            var role = _roleManager.FindByIdAsync(id).Result;
            if (role is not null)
            {
                var result = _roleManager.DeleteAsync(role).Result;
            }
            else
            {
                ViewBag.error = "Role ismi bulunamadı";
            }

            return RedirectToAction("Roles");
        }

        public IActionResult RoleUpdate(string id)
        {
            var role = _roleManager.FindByIdAsync(id).Result;

            if (role is null)
            {
                return RedirectToAction("Roles");
            }

            return View(role.Adapt<RoleViewModel>());
        }

        [HttpPost]
        public IActionResult RoleUpdate(RoleViewModel roleViewModel)
        {
            var role = _roleManager.FindByIdAsync(roleViewModel.Id).Result;

            if (role is not null)
            {
                role.Name = roleViewModel.Name;
                var result = _roleManager.UpdateAsync(role).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Güncelleme işlemi başarısız");
            }

            return View(roleViewModel);
        }

        public IActionResult RoleAssign(string id)
        {
            TempData["userId"] = id;

            var user = _userManager.FindByIdAsync(id).Result;
            ViewBag.userName = user.UserName;

            var roles = _roleManager.Roles;
            var userRoles = _userManager.GetRolesAsync(user).Result;

            var listOfRoleAssignViewModels = new List<RoleAssignViewModel>();
            foreach (var role in roles)
            {
                var roleAssignViewModel = new RoleAssignViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                roleAssignViewModel.Exist = userRoles.Contains(role.Name);

                listOfRoleAssignViewModels.Add(roleAssignViewModel);
            }

            return View(listOfRoleAssignViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignViewModel> roleAssignViewModels)
        {
            var user = _userManager.FindByIdAsync(TempData["userId"].ToString()).Result;


            foreach (var model in roleAssignViewModels)
            {
                if (model.Exist)
                {
                    _userManager.AddToRoleAsync(user, model.RoleName).Wait();
                }
                else
                {
                    _userManager.RemoveFromRoleAsync(user, model.RoleName).Wait();
                }
            }

            return RedirectToAction("Users");
        }
    }
}