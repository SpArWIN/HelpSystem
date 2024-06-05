using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAccountService _accountService;
        //  private readonly IUserService _userService;
        public AdminController(IAccountService service)
        {
            _accountService = service;
            // _userService = userService;
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(Roles = "2")]
        [HttpGet]
        public IActionResult AdminPanel()
        {

            return View();
        }



    }
}
