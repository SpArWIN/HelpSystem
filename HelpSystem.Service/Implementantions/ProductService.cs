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
       
        private readonly IBaseRepository<Products> _productsRepository;
        private readonly IBaseRepository<Warehouse> _warehouseRepository;
        private readonly IBaseRepository<Provider> _providerRepository;

        public ProductService(IBaseRepository<Products> productsRepository, IBaseRepository<Warehouse> waRepository, IBaseRepository<Provider> provideRepository)
        {
          
            _productsRepository = productsRepository;
            _warehouseRepository = waRepository;
            _providerRepository = provideRepository;
        }

        public Task<BaseResponse<IEnumerable<ProductShowViewModel>>> GetAllProduct()
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse<IEnumerable<Products>>> CreateProduct(List<ProductViewModel> positions)
        {
            try
            {
                var productsList = new List<Products>();

                foreach (var pos in positions)
                {
                    var Warehouse = await _warehouseRepository.GetAll()
                        .FirstOrDefaultAsync(x => x.Id == pos.WarehouseId);
                    var Provider = await _providerRepository.GetAll()
                        .FirstOrDefaultAsync(x => x.Id == pos.ProviderID);

                    // Получаем количество товаров, которые нужно создать
                    int quantity = int.Parse(pos.Quantity);

                    for (int i = 0; i < quantity; i++)
                    {
                        var NewProduct = new Products
                        {
                            InventoryCode = pos.InventoryCode,
                            NameProduct = pos.NameProduct,
                            Comments = pos.Comments,
                            Provider = Provider,
                            Warehouse = Warehouse,
                            UserId = null,
                            User = null
                        };

                        await _productsRepository.Create(NewProduct);
                        productsList.Add(NewProduct);
                    }
                }

                return new BaseResponse<IEnumerable<Products>>()
                {
                    Data = productsList,
                    Description = "Товары успешно добавлены",
                    StatusCode = StatusCode.Ok
                };
            



            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<Products>>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
