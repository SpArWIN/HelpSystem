using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProviderService _providerService;
        private readonly IWarehouseService _warehouseService;
        public ProductController(IProductService productService, IProviderService provider, IWarehouseService warehouse)
        {
            _productService = productService;
            _providerService = provider;
            _warehouseService = warehouse;
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var ProvTask = _providerService.GetAllProvider();
            var WarehousTask = _warehouseService.GetAllWarehouse();

            await Task.WhenAll(ProvTask, WarehousTask); // Ожидание завершения обеих задач

            ViewBag.Providers = ProvTask.Result;
            ViewBag.Warehouses = WarehousTask.Result;

            //  return PartialView("_PartialProduct");
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
