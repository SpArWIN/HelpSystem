﻿using HelpSystem.Domain.ViewModel.Provider;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    public class ProviderController : Controller
    {
        private readonly IProviderService _providerService;

        public ProviderController(IProviderService service)
        {
            _providerService = service;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        //Метод действия создания поставщика
        public async Task<IActionResult> CreateProv(ProviderViewModel provider)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                var errorDescription = string.Join(",", errors);
                return BadRequest(new { description = errorDescription });
            }
            var Response = await _providerService.CreateProvider(provider);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { Response.Data, description = Response.Description });
            }

            return BadRequest(new { description = Response.Description });
        }
    }
}
