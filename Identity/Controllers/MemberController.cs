using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[Authorize]
public class MemberController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}