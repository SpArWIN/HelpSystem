using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
