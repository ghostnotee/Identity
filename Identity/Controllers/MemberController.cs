using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

public class MemberController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}