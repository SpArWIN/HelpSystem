using HelpSystem.Domain.ViewModel.Account;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ITokenCacheService _tokenCacheService;
        public AccountController(IAccountService accountService, ITokenCacheService tokenCacheService)
        {
            _accountService = accountService;
            _tokenCacheService = tokenCacheService;
        }




        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVIewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                var errorDescription = string.Join(",", errors);
                return BadRequest(new { Description = errorDescription });
            }

            var Response = await _accountService.Register(model);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(Response.Data));

                return RedirectToAction("Index", "Home", new { Description = Response.Description });
            }

            return BadRequest(new { Description = Response.Description });


        }
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                var errorDescription = string.Join(",", errors);
                return BadRequest(new { Description = errorDescription });
            }

            var Response = await _accountService.Login(model);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(Response.Data));


                return RedirectToAction("Index", "Home", new { Description = Response.Description });
            }

            return BadRequest(new { Description = Response.Description });

        }

        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> RecoveryPassword(Guid UserId, string Token)
        {
            //Проверим, есть ли она уже в кеше
            var Tok = await _tokenCacheService.GetTokenAsync(Token);
            if (Tok.StatusCode == Domain.Enum.StatusCode.NotFind)
            {
                await Logout();
                return View("ErrorRecovery", Tok.Description);
            }
            //Через какое время токен из кеша должен пропасть


            ViewBag.UserId = UserId;
            ViewBag.Token = Token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoveryPassword(RecoveryProfile model, string Token)
        {
            var Response = await _accountService.RecoveryPassword(model, Token);
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                var errorDescription = string.Join(",", errors);
                return BadRequest(new { Description = errorDescription });
            }


            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { description = Response.Description });
            }
            return BadRequest(new { description = Response.Description });
        }
    }

}
