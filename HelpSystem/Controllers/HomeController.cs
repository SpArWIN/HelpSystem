using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Extension;
using HelpSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace HelpSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }

        public IActionResult Index()
        {
            var roleIdCookie = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Проверяем, удалось ли получить ID роли и преобразовать его в целое число
            if (int.TryParse(roleIdCookie, out int roleId))
            {

                var role = ((UserRoleType)roleId).GetDisplayName();


                ViewData["UserRole"] = role;
            }

            return View();



        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
