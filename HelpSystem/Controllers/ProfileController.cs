using HelpSystem.Domain.ViewModel.Profile;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpSystem.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;
        public ProfileController(IProfileService profileService, IUserService userService)
        {
            _profileService = profileService;
            _userService = userService;
        }


        public async Task<IActionResult> Detail(Guid? id)
        {
            //Тут или мы переходим на свой профиль или админ переходит по профилям

            if (id == null)
            {


                var currentUser = HttpContext.User;
                var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    // Вызов метода с идентификатором пользователя
                    var response = await _profileService.GetProfile(userId);
                    if (response.StatusCode == Domain.Enum.StatusCode.Ok)
                    {
                        return View(response.Data);
                    }
                }
            }
            var Response = await _profileService.GetProfile(id.Value);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return View(Response.Data);
            }

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetProfile(Guid id, bool isJson)
        {
            var response = await _profileService.GetProfile(id);

            if (isJson)
            {
                return Json(response.Data);
            }
            return PartialView("_PartialDetail", response.Data);


        }
        [Authorize(Roles = "3")]
        public async Task<IActionResult> GetUsers()
        {
            var Response = await _userService.GetAllUsers();
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return View(Response.Data);
            }

            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Save(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                var errorDescription = string.Join(",", errors);
                return BadRequest(new { description = errorDescription });
            }
            var Response = await _profileService.Save(model);

            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {

                return Ok(new { description = Response.Description });
            }
            else
            {
                return BadRequest(new { description = Response.Description });
            }
        }



        [HttpPost]
        public async Task<IActionResult> GetUsers(string term)
        {
            var Response = await _profileService.GetUser(term);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Json(Response.Data);
            }
            return Json(Response.Data);
        }
    }
}
