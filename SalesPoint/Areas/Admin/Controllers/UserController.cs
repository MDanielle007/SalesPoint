using Microsoft.AspNetCore.Mvc;

namespace SalesPoint.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Areas/Admin/Views/User/Index.cshtml");
        }
    }
}
