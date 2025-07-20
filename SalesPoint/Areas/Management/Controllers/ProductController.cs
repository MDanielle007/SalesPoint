using Microsoft.AspNetCore.Mvc;

namespace SalesPoint.Areas.Management.Controllers
{
    [Area("Management")]
    public class ProductController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Areas/Management/Views/Product/Index.cshtml");
        }
    }
}
