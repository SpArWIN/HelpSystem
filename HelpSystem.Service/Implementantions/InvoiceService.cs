using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Invoice;
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

        public async Task<DataTableResponse> GetAllInvoices()
        {
            try
            {
                var QueryResponse = await _invoiceRepository.GetAll()
                    .Select(x => new InvoiceShowViewModel()
                    {
                        Id = x.Id,
                        DateCreated = x.CreationDate.ToString("dd.MM.yyyy HH:mm"),
                        NumberInvoice = x.NumberDocument
                    }).ToListAsync();
                //Пока в total 0
                return new DataTableResponse()
                {
                    Data = QueryResponse,
                    Total = 0

                };
            }
            catch (Exception ex)
            {
                return new DataTableResponse()
                {
                    Data = null,
                    Total = 0
                };
            }
        }
        /// <summary>
        /// Метод нацелен на возвращение списков товаров, связанных с этой накладной
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IBaseResponse<IEnumerable<ProductShowViewModel>>> GetPartialProduct(Guid id)
        {
            try
            {
                //Находим накладную по ее id
                var Invoice = await _invoiceRepository.GetAll()
                    .Include(p => p.Products)
                    .ThenInclude(w => w.Provider)
                    .Include(p => p.Products)
                    .ThenInclude(w => w.Warehouse)

                    .FirstOrDefaultAsync(x => x.Id == id);
                if (Invoice != null)
                {
                    var productsList = Invoice.Products.ToList();

                    string Numbers = Invoice.NumberDocument;
                    var Products = productsList
                        .GroupBy(p => p.NameProduct)
                        .Select(g => new ProductShowViewModel()
                        {
                            
                            NameProduct = g.Key,
                            CodeProduct = g.FirstOrDefault().InventoryCode,    
                           Warehouse = g.FirstOrDefault().Warehouse.Name,
                           Provider = g.FirstOrDefault().Provider.Name,
                           NumberDoc = Numbers,
                            TotalCount = g.Count()
                        }).ToList();
                    return new BaseResponse<IEnumerable<ProductShowViewModel>>()
                    {
                        Data = Products,
                        StatusCode = StatusCode.Ok
                    };
                }

                return new BaseResponse<IEnumerable<ProductShowViewModel>>()
                {
                    StatusCode = StatusCode.NotFind,
                    Description = "Накладная не найдена",

                };
            }
            catch (Exception e)
            {
                return new BaseResponse<IEnumerable<ProductShowViewModel>>()
                {
                    Description = $"{e.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
