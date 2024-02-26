using HelpSystem.Domain.ViewModel.Statment;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpSystem.Controllers
{
    public class StatmentController : Controller
    {
        private readonly IStatmentIService _statmentIService;

        public StatmentController(IStatmentIService statment)
        {
            _statmentIService = statment;
        }
        [HttpGet]
        public IActionResult CreateStatment() => View();

        [HttpPost]
        public async Task<IActionResult> CreateStatment(StatmentViewModel model)
        {
           
            var currentUser = HttpContext.User;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                var errorDescription = string.Join(",", errors);
                return BadRequest(new { description = errorDescription });
            }
            var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                //Завтра доделай это
                var Response = await _statmentIService.CreateStatment(model, userId);
                if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
                {
                    return Ok();
                }
                return BadRequest( new  {description = Response.Description});
            }

            return View();

        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
