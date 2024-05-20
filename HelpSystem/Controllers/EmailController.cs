
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailSender _emailSender;
        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PasswordRecovery(string Email)
        {
            var Response = await _emailSender.RecoveryPassword(Email);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { description = Response.Description });

            }
            return BadRequest(new { description = Response.Description });
        }
    }
}
