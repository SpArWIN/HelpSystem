using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    public class WarehouseController : Controller
    {
        //Главная страница для склада
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
