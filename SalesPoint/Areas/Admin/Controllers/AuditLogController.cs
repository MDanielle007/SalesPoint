using Microsoft.AspNetCore.Mvc;

namespace SalesPoint.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuditLogController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Areas/Admin/Views/AuditLog/Index.cshtml");
        }
    }
}
