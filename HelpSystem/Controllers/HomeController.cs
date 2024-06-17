using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Extension;
using HelpSystem.Models;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace HelpSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProfileService _profileService;
        public HomeController(ILogger<HomeController> logger, IProfileService profile)
        {
            _logger = logger;
            _profileService = profile;
        }

        public async Task<IActionResult> Index()
        {
            var roleIdCookie = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var UserClaimId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (UserClaimId != null)
            {
                Guid UserId;
                if (Guid.TryParse(UserClaimId.Value, out UserId))
                {
                    var Response = await _profileService.GetProfile(UserId);
                    if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
                    {

                        // Проверяем, удалось ли получить ID роли и преобразовать его в целое число
                        if (int.TryParse(roleIdCookie, out int roleId))
                        {

                            var role = ((UserRoleType)roleId).GetDisplayName();


                            ViewData["UserRole"] = role;
                        }



                        return View(Response.Data);
                    }

                }
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
