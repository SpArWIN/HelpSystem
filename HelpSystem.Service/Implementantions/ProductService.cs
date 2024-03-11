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
        //private readonly IBaseRepository<Invoice> _invoiceRepository;
        private readonly IBaseRepository<Products> _productsRepository;
        private readonly IBaseRepository<Warehouse> _warehouseRepository;
        private readonly IBaseRepository<Provider> _providerRepository;

        public ProductService(IBaseRepository<Products> productsRepository, IBaseRepository<Warehouse> waRepository, IBaseRepository<Provider> provideRepository, IBaseRepository<Invoice> invoiceRepository)
        {

            _productsRepository = productsRepository;
            _warehouseRepository = waRepository;
            _providerRepository = provideRepository;
            //_invoiceRepository = invoiceRepository;
        }




        public async Task<BaseResponse<IEnumerable<Products>>> CreateProduct(List<ProductViewModel> positions)
        {
            try
            {
                var productsList = new List<Products>();
                var SuccessProductList = new List<Products>();
                //Добавлю индекс для позиции, которая будет указывать пользователю в какой конкретной позиции не указано что -то
                int positionIndex = 0;

                if (positions.Count > 0)
                {



                    foreach (var pos in positions)
                    {
                        var Warehouse = await _warehouseRepository.GetAll()
                            .FirstOrDefaultAsync(x => x.Id == pos.WarehouseId);
                        var Provider = await _providerRepository.GetAll()
                            .FirstOrDefaultAsync(x => x.Id == pos.ProviderID);

                        if (Warehouse == null)
                        {
                            return new BaseResponse<IEnumerable<Products>>()
                            {
                                Description = "Склад не найден",
                                StatusCode = StatusCode.NotFind
                            };
                        }

                        if (Provider == null)
                        {
                            return new BaseResponse<IEnumerable<Products>>()
                            {
                                Description = "Поставщик не найден",
                                StatusCode = StatusCode.NotFind
                            };
                        }
                        positionIndex++;

                        if (string.IsNullOrEmpty(pos.Quantity) || pos.Quantity == "0")
                        {
                            return new BaseResponse<IEnumerable<Products>>()
                            {
                                Description = $"В {positionIndex} или не указано количество или оно меньше 0",
                                StatusCode = StatusCode.UnCreated
                            };
                        }
                        // Получаем количество товаров, которые нужно создать
                        int quantity = int.Parse(pos.Quantity);
                        if (string.IsNullOrEmpty(pos.NameProduct))
                        {
                            return new BaseResponse<IEnumerable<Products>>()
                            {
                                Description = $"Наименование товара не указано в {positionIndex} позиции",
                                StatusCode = StatusCode.UnCreated
                            };
                        }

                        if (string.IsNullOrEmpty(pos.InventoryCode))
                        {
                            return new BaseResponse<IEnumerable<Products>>()
                            {
                                Description = $"Инвентарный код не указан в {positionIndex} позиции",
                                StatusCode = StatusCode.UnCreated
                            };
                        }
                        /*
                         * Тут есть косяк, если на следующей итерации будет проблема, предыдущий товар уже будет создан,
                         * а значит, он не будет ни к чему прикреплен, решается это добавлением ещё одного цикла)
                         */


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

                            productsList.Add(NewProduct);
                        }



                    }
                    // Добавляем все успешно проверенные товары в базу данных
                    foreach (var product in productsList)
                    {
                        await _productsRepository.Create(product);
                    }

                    return new BaseResponse<IEnumerable<Products>>()
                    {
                        Data = productsList,
                        Description = "Товары успешно добавлены",
                        StatusCode = StatusCode.Ok
                    };
                }

                return new BaseResponse<IEnumerable<Products>>()
                {
                    Description = "Количество позиций меньше 0",
                    StatusCode = StatusCode.UnCreated
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
