﻿using HelpSystem.Domain.ViewModel.Product;
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
    }
}
