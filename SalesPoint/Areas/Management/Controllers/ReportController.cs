using Microsoft.AspNetCore.Mvc;

namespace SalesPoint.Areas.Management.Controllers
{
    [Area("Management")]
    public class ReportController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Areas/Management/Views/Report/Index.cshtml");
        }
    }
}
