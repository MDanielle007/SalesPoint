using Microsoft.AspNetCore.Mvc;

namespace SalesPoint.Areas.Management.Controllers
{
    [Area("Management")]
    public class CategoryController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Areas/Management/Views/Category/Index.cshtml");
        }
    }
}
