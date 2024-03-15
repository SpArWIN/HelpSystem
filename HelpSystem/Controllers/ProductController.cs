using System.Linq.Expressions;
using HelpSystem.Domain.ViewModel.Product;
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

        //[HttpGet]
        //public async Task<IActionResult> CreateProduct()
        //{
        //    var ProvTask = _providerService.GetAllProvider();
        //    var WarehousTask = _warehouseService.GetAllWarehouse();

        //    await Task.WhenAll(ProvTask, WarehousTask); // Ожидание завершения обеих задач

        //    ViewBag.Providers = ProvTask.Result;
        //    ViewBag.Warehouses = WarehousTask.Result;

        //    //  return PartialView("_PartialProduct");
        //    return View();
        //}


        //Метод когда получаем товары, пользователь то уже есть
        public async Task<IActionResult> GetProduct(string term)
        {
            var Response = await _productService.GetProduct(term);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Json(Response.Data);
            }
            return Json(Response.Description);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> BindingProduct(Guid StatId, Guid ProductId, string? Comments)
        {
            var Response = await _productService.BindingProduct(StatId, ProductId,Comments);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { Response.Data, description = Response.Description });

            }

            return BadRequest(new { description = Response.Description });
        }

        
        //Снятие с пользователя товара
        [HttpPost]
        public async Task<IActionResult> UnbindingProduct(UnbindingProductViewModel model)
        {
            var Response = await _productService.UnBindingProduct(model);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { Response.Data, description = Response.Description });
            }

            return BadRequest(new { description = Response.Description });
        }
    }
}
