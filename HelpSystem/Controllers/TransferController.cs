using HelpSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpSystem.Controllers
{
    public class TransferController : Controller
    {
        private readonly ITransferService _transferService;

        public TransferController(ITransferService transfer)
        {
            _transferService = transfer;
        }

       
        public IActionResult Index()
        {
            return View();
        }
    }
}
