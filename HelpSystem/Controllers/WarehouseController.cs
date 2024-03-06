using HelpSystem.Domain.ViewModel.Warehouse;
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

        public IActionResult Index()
        {
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
    }
}
