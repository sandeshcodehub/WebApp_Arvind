using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppMvc.Areas.Secure.Controllers
{
    [Area("Secure")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
