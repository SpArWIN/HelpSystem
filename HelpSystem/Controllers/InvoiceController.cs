using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IProviderService _providerService;
        private readonly IWarehouseService _warehouseService;

        public InvoiceController(IProviderService pro, IWarehouseService warehouseService,IInvoiceService service)
        {
            _providerService = pro;
            _warehouseService = warehouseService;
            _invoiceService = service;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = new ProductViewModel();
            var response = await _providerService.GetAllProvider();
            var Warehouse = await _warehouseService.GetAllWarehouse();

            if (response.StatusCode == Domain.Enum.StatusCode.Ok && Warehouse.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                viewModel.Providers = response.Data;
                viewModel.Warehouses = Warehouse.Data;

            }
            else
            {
                return View(viewModel);
            }

            
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreateInvoiceProducts( List<ProductViewModel> positions, string Number)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                var errorDescription = string.Join(",", errors);
                return BadRequest(new { description = errorDescription });
            }
            var Response = await _invoiceService.CreateInvoice(Number, positions);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { message = Response.Description });
            }

            return BadRequest(new { description = Response.Description });
        }
    }
}
