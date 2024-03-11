using HelpSystem.Domain.ViewModel.Warehouse;
using HelpSystem.Service.Implementantions;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouse)
        {
            _warehouseService = warehouse;
        }

        //Главная страница для склада

        public async Task<IActionResult> Index()
        {
            var Response = await _warehouseService.GetAllWarehouse();
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return View(Response.Data);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateWarehouse(WarehouseViewModel model)
        {
            var Response = await _warehouseService.CreateWarehouse(model);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { Response.Data, description = Response.Description });
            }

            return BadRequest(new { description = Response.Description });
        }

        public async Task<IActionResult> GetCurrentWarehouse(Guid id)
        {

            var Response = await _warehouseService.GetWarehouse(id);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return PartialView("_PartialWarehouse", Response.Data);
            }

            return PartialView("_PartialWarehouse");

        }

        public async Task<IActionResult> UpdateWarehouse(WarehouseViewModel model)
        {
            var Response = await _warehouseService.SaveWarehouse(model);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { Response.Data, description = Response.Description });
            }

            return BadRequest(new { description = Response.Description });
        }
    }
}
