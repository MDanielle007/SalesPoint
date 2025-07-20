using Microsoft.AspNetCore.Mvc;

namespace SalesPoint.Areas.Management.Controllers
{
    [Area("Management")]
    public class DashboardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Areas/Management/Views/Dashboard/Index.cshtml");
        }
    }
}
