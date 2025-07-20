using Microsoft.AspNetCore.Mvc;

namespace SalesPoint.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class TransactionController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Areas/Sales/Views/Transaction/Index.cshtml");
        }

        [HttpGet]
        public IActionResult PointOfSales()
        {
            return View("~/Areas/Sales/Views/Category/PointOfSales.cshtml");
        }
    }
}
