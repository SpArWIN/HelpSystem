using HelpSystem.Domain.ViewModel.Provider;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    [Authorize]
    public class ProviderController : Controller
    {
        private readonly IProviderService _providerService;

        public ProviderController(IProviderService service)
        {
            _providerService = service;
        }
        //В index сразу передаем всех поставщиков
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Response = await _providerService.GetAllProvider();
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return View(Response.Data);
            }

            return View();
        }

        public async Task<IActionResult> GetCurrentProvider(Guid id)
        {
            var CurProvider = await _providerService.GetProviderCurrent(id);

            if (CurProvider.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return PartialView( "_PartialProvider", CurProvider.Data);
            }

            return PartialView("_PartialProvider");
        }

        //Метод обновления поставщика 
        public async Task<IActionResult> UpdateProvider(ProviderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                var errorDescription = string.Join(",", errors);
                return BadRequest(new { description = errorDescription });
            }
            var Ups = await _providerService.SaveProvider(model);
            if (Ups.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { Ups.Data, description = Ups.Description });
            }

            return BadRequest(new { description = Ups.Description });
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
        //Метод для получения списка всез поставщиков  для отображения при  создании товара

    }
}
