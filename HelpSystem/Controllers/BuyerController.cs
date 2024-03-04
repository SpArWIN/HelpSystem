using HelpSystem.DAL.Interfasces;
using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    public class BuyerController : Controller
    {
        private readonly IBuyerService _buyerService;
        public BuyerController(IBuyerService buyerService)
        {
            _buyerService = buyerService;
        }
        //Сюда тоже сразу передаем список всех покупателей
        public async Task< IActionResult> Index()
        {
            var Response = await _buyerService.GetAllBuyer();
            if (Response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return View(Response.Data);
            }

            return View();
        }
    }
}
