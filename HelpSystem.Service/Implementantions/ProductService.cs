using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class ProductService : IProductService
    {
       private readonly IInvoiceService _invoiceService;
        private readonly IBaseRepository<Products> _productsRepository;
        private readonly IBaseRepository<Warehouse> _warehouseRepository;
        private readonly IBaseRepository<Provider> _providerRepository;

        public ProductService(IInvoiceService invoiceRepository, IBaseRepository<Products> productsRepository,IBaseRepository<Warehouse> waRepository,IBaseRepository<Provider> provideRepository)
        {
            _invoiceService = invoiceRepository;
            _productsRepository = productsRepository;
           _warehouseRepository = waRepository;
           _providerRepository = provideRepository;
        }

        public Task<BaseResponse<IEnumerable<ProductShowViewModel>>> GetAllProduct()
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse<Products>> CreateProduct(string NumberDocument, List<ProductViewModel> positions)
        {
            try
            {
                var NewNaklad = await _invoiceService.CreateInvoice(NumberDocument);
                if (NewNaklad.StatusCode == StatusCode.Ok)
                {
                   
                    
                    foreach (var pos in positions)
                    {
                        var Warehouse = await _warehouseRepository.GetAll()
                            .FirstOrDefaultAsync(x => x.Id == pos.WarehouseId);
                        var Provider = await _providerRepository.GetAll()
                            .FirstOrDefaultAsync(x => x.Id == pos.ProviderID);

                        var NewProduct = new Products
                        {

                            InventoryCode = pos.InventoryCode,
                            NameProduct = pos.NameProduct,
                            Comments = pos.Comments,
                            Provider = Provider,
                            Warehouse = Warehouse,
                            UserId = null,
                            User = null,

                        };
                        await _productsRepository.Create(NewProduct);

                    }

                    return new BaseResponse<Products>()
                    {
                        Data = null,
                        Description = "Товары успешно добавлены в накладную",
                        StatusCode = StatusCode.Ok
                    };
                }

                return new BaseResponse<Products>()
                {
                    StatusCode = NewNaklad.StatusCode,
                    Description = NewNaklad.Description
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Products>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
