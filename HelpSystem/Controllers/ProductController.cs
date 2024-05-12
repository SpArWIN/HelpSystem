using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        //Метод получения всей доступной информации о товаре 
        public async Task<IActionResult> GetAllInfoProduct(int Id)
        {
            var Response = await _productService.GetMainProductInfo(Id);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { data = Response.Data, description = Response.Description });
            }

            return BadRequest(new { description = Response.Description });
        }
        //Метод когда получаем товары, пользователь то уже есть - будет искать товары, которые не привязаны к юзверю
        public async Task<IActionResult> GetProduct(string term)
        {
            var Response = await _productService.GetProductNotUser(term);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Json(Response.Data);
            }
            return Json(Response.Description);
        }


        public async Task<IActionResult> GetAllproducts(string term)
        {
            var Response = await _productService.GetAllProductsWith(term);
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
        //Привязка товара к пользователю со стороны заявки
        public async Task<IActionResult> BindingProduct(Guid StatId, int ProductId, string? Comments)
        {
            var Response = await _productService.BindingProduct(StatId, ProductId, Comments);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { Response.Data, description = Response.Description });

            }

            return BadRequest(new { description = Response.Description });
        }


        //Снятие с пользователя товара
        [HttpPost]
        public async Task<IActionResult> UnbindingProduct(List<UnbindingProductViewModel> model, Guid ProfileId)
        {
            var Response = await _productService.UnBindingProduct(model, ProfileId);
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { Response.Data, description = Response.Description });
            }

            return BadRequest(new { description = Response.Description });
        }


    }
}
