﻿using HelpSystem.Domain.ViewModel.Profile;
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


        public async Task<IActionResult> Detail()
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

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetProfile(Guid id, bool isJson)
        {
            var Response = await _profileService.GetProfile(id);
            if (isJson)
            {
                return Json(Response.Data);
            }
            return PartialView("_PartialDetail", Response.Data);


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
                return BadRequest(new { message = errorDescription });
            }
            var Response = await _profileService.Save(model);

            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { Response.Data, message = Response.Description });
            }

            //  return Json(new { message = Response.Description });
            return BadRequest(new { message = Response.Description });
        }
    }
}
