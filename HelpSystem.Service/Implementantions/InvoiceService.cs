using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IBaseRepository<Invoice> _invoiceRepository;
        private readonly IProductService _productService;

        public InvoiceService(IBaseRepository<Invoice> invoiceRepository, IProductService productService)
        {
            _invoiceRepository = invoiceRepository;
            _productService = productService;
        }
        public async Task<IBaseResponse<Invoice>> CreateInvoice(string NumberDocument, List<ProductViewModel> positions)
        {
            try
            {
                var Response = await _invoiceRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.NumberDocument == NumberDocument);
                if (Response != null)
                {
                    return new BaseResponse<Invoice>()
                    {
                        Description = "Накладная с таким номером уже существует",
                        StatusCode = StatusCode.UnCreated
                    };
                }

                if (string.IsNullOrEmpty(NumberDocument))
                {
                    return new BaseResponse<Invoice>()
                    {
                        Description = "Не указан номер документа",
                        StatusCode = StatusCode.UnCreated
                    };
                }
                var Invoice = new Invoice()
                {
                    CreationDate = DateTime.Now,
                    NumberDocument = NumberDocument,

                };
                //Создаем накладную


                //Добавляем товары
                var ProductResponse = await _productService.CreateProduct(positions);

                if (ProductResponse.StatusCode == StatusCode.Ok)
                {
                    await _invoiceRepository.Create(Invoice);
                    //Привязываем товары к накладной, которую создали
                    Invoice.Products = ProductResponse.Data.ToList();
                    await _invoiceRepository.Update(Invoice);

                    return new BaseResponse<Invoice>()
                    {
                        Data = Invoice,
                        Description = $"Товарная накладная с номером {NumberDocument} успешно создана ",
                        StatusCode = StatusCode.Ok
                    };
                }


                return new BaseResponse<Invoice>()
                {
                    Description = ProductResponse.Description,
                    StatusCode = ProductResponse.StatusCode
                };

            }
            catch (Exception e)
            {
                return new BaseResponse<Invoice>()
                {
                    Description = $"{e.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
