using HelpSystem.DAL.Implementantions;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HelpSystem.Service.Implementantions
{
    public class ProductService : IProductService
    {
        //private readonly IBaseRepository<Invoice> _invoiceRepository;
        private readonly IBaseRepository<Products> _productsRepository;
        private readonly IBaseRepository<Warehouse> _warehouseRepository;
        private readonly IBaseRepository<Provider> _providerRepository;
        private readonly IBaseRepository<Statement> _statementRepository;
        private readonly IBaseRepository<ProductMovement> _productMovementRepository;
        public ProductService(IBaseRepository<Products> productsRepository, IBaseRepository<Warehouse> waRepository, IBaseRepository<Provider> provideRepository, IBaseRepository<Statement> statement, IBaseRepository<ProductMovement> productMovementRepository)
        {

            _productsRepository = productsRepository;
            _warehouseRepository = waRepository;
            _providerRepository = provideRepository;
            //_invoiceRepository = invoiceRepository;
            _statementRepository = statement;
            _productMovementRepository = productMovementRepository;
        }

        /// <summary>
        /// Метод получения товара по его имени или артикулу
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public async Task<BaseResponse<Dictionary<Guid, string>>> GetProduct(string term)
        {
            try
            {

                var productsInMemory = await _productsRepository.GetAll()
                    .Include(w => w.Warehouse)
                    .Where(x => x.UserId == null) // Проверяем, что товар не привязан к пользователю
                    .Where(x => EF.Functions.Like(x.NameProduct, $"%{term}%") || EF.Functions.Like(x.InventoryCode, $"%{term}%"))
                    .ToListAsync(); 


              
                    


                if (productsInMemory.Any()) // Проверяем, найдены ли товары
                {

                    var Products = new Dictionary<string, Dictionary<Guid, string>>();
                    //Теперь пробежимся по всем получившим товарам и соориентируемся где они
                    // Разгруппируем товары по наименованию
                    var groupedProductsByName = productsInMemory.GroupBy(x => x.NameProduct);

                    // Проходим по каждой группе товаров с одинаковым наименованием
                    foreach (var groupByName in groupedProductsByName)
                    {
                        var productsByLocation = new Dictionary<Guid, string>(); // Словарь для товаров на текущем складе
                        var movedProductsByLocation = new Dictionary<Guid, string>(); // Словарь для перемещенных товаров

                        foreach (var prod in groupByName)
                        {
                            var destinationWarehouseName = prod.Warehouse.Name;

                            // Находим последнее перемещение товара
                            var lastMovement = await _productMovementRepository.GetAll()
                                .OrderByDescending(pm => pm.MovementDate)
                                .FirstOrDefaultAsync(pm => pm.ProductId == prod.Id);

                            if (lastMovement != null)
                            {
                                // Получаем наименование склада назначения
                                destinationWarehouseName = await _warehouseRepository.GetAll()
                                    .Where(x => x.Id == lastMovement.DestinationWarehouseId)
                                    .Select(x => x.Name)
                                    .FirstOrDefaultAsync();

                                // Проверяем, перемещен ли товар на другой склад
                                if (lastMovement.DestinationWarehouseId != prod.Warehouse.Id)
                                {
                                    var productInfo = $"{prod.NameProduct} ({prod.InventoryCode}) Склад: {destinationWarehouseName}";
                                    movedProductsByLocation[prod.Id] = productInfo;
                                    continue; // Продолжаем цикл, чтобы избежать добавления товара в productsByLocation
                                }
                            }

                            // Формируем строку с информацией о товаре и его местоположении на текущем складе
                            var productInfoOnCurrentWarehouse = $"{prod.NameProduct} ({prod.InventoryCode}) Склад: {destinationWarehouseName}";
                            productsByLocation[prod.Id] = productInfoOnCurrentWarehouse;
                        }

                        // Добавляем товары на текущем складе в словарь продуктов
                        Products[groupByName.Key] = productsByLocation;

                        // Добавляем товары, перемещенные на другие склады, в словарь продуктов
                        if (movedProductsByLocation.Any())
                        {
                            foreach (var movedProduct in movedProductsByLocation)
                            {
                                Products[movedProduct.Value] = new Dictionary<Guid, string> { { movedProduct.Key, movedProduct.Value } };

                            }
                        }
                    }

                    var UnionProducts = Products
                        .GroupBy(x => x.Value)
                        .Select(x => x.Key.First())
                        .Select(x => new
                        {
                            x.Key,
                            x.Value
                        })
                        .ToDictionary(x => x.Key, x => x.Value);
                    //Завтра переделать.
                    return new BaseResponse<Dictionary<Guid, string>>()
                    {
                        StatusCode = StatusCode.Ok,
                        Data = UnionProducts
                    };

                  


                }
                else // Если товары не найдены, вернем специальное значение
                {
                    return new BaseResponse<Dictionary<Guid, string>>()
                    {
                        StatusCode = StatusCode.NotFind,
                        Description = "Товары не найдены"
                    };
                }

            }
            catch (Exception ex)
            {
                return new BaseResponse<Dictionary<Guid, string>>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<Products>>> CreateProduct(List<ProductViewModel> positions)
        {
            try
            {
                var productsList = new List<Products>();
               
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
                                Description = $"В позиции {positionIndex} или не указано количество или оно меньше 0",
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

        public async Task<BaseResponse<Products>> BindingProduct( Guid StatId ,Guid ProductId,string? Comments)
        {
            try
            {
                var User = await _statementRepository.GetAll()
                    .Include(u => u.User)
                    .Where(stat => stat.ID== StatId)
                    .Select(stat => stat.User.Id)
                    .FirstOrDefaultAsync(); 


                var Product = await _productsRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == ProductId);
                if (Product != null)
                {
                    if (Product.UserId != null)
                    {
                        return new BaseResponse<Products>()
                        {
                            Description =
                                $"{Product.NameProduct} ({Product.InventoryCode}) \nуже связан спользователем",
                            StatusCode = StatusCode.UnChanched
                        };
                    }
                    Product.UserId = User;
                    Product.Comments = Comments;
                    await _productsRepository.Update(Product);
                    return new BaseResponse<Products>()
                    {
                        Data = Product,
                        StatusCode = StatusCode.Ok,
                        Description = $"Связь успешно установлена"
                    };

                }

                return new BaseResponse<Products>()
                {
                    Description = $"Товар не найден",
                    StatusCode = StatusCode.NotFind
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse<Products>()
                {
                    StatusCode = StatusCode.InternalServerError,
                    Description = $"{ex.Message}"
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<Products>>> UnBindingProduct(UnbindingProductViewModel product)
        {
            try
            {
                //Находим пользователя, а именно его фио
                var Profile = await _productsRepository
                    .GetAll()
                    .Where(x => x.UserId == product.ProfileId)
                    .Select(x => new
                    {
                        FullName = x.User.Profile.LastName + " " + x.User.Profile.Name + " " + x.User.Profile.Surname
                    })
                    .FirstOrDefaultAsync();
                    
                if (Profile != null)
                {

                    //Количество запрашиваемого товара, который необходимо снять
                    int RequestProductCount = product.CountUnbinding;
                    if (RequestProductCount > 0)
                    {
                        var productsToUnbind = await _productsRepository.GetAll()
                            .Where(x => x.UserId== product.ProfileId)
                            .Where(x => x.NameProduct == product.NameProduct && x.InventoryCode == product.Code)
                            .Take(RequestProductCount) // Взять запрошенное количество товаров для снятия привязки
                            .ToListAsync();
                            //Предположим, что у пользователя 5 катриджей, а просят снять с него 8, косяк, получается.

                        if (productsToUnbind.Count < RequestProductCount)
                        {
                            return new BaseResponse<IEnumerable<Products>>()
                            {
                                Description = "Запрошено снятие привязки большего количества товаров, чем доступно",
                                StatusCode = StatusCode.UnChanched,
                            
                            };
                        }
                        else
                        {
                            //Снятие происходит путём очищения привязки пользователя
                            foreach (var prod in productsToUnbind)
                            {
                                prod.UserId = null;
                                prod.Comments = $"Последний владелец: {Profile.FullName}";
                                await _productsRepository.Update(prod);
                            }

                            string description;
                            int lastDigit = RequestProductCount % 10;
                            if (RequestProductCount >= 11 && RequestProductCount <= 14)
                            {
                                description =
                                    $"{RequestProductCount} товаров {product.NameProduct} ({product.Code}) \n было снято с {Profile.FullName}";

                            }
                            else if (lastDigit == 1)
                            {
                                description =
                                    $"{RequestProductCount} товар: {product.NameProduct} ({product.Code}) \n был снят с {Profile.FullName}";
                            }
                            else if (lastDigit >= 2 && lastDigit <= 4)
                            {
                                description =
                                    $"{RequestProductCount} товара : {product.NameProduct} ({product.Code}) \n было снято с {Profile.FullName}";
                            }
                            else
                            {
                                description =
                                    $"{RequestProductCount} товаров : {product.NameProduct} ({product.Code}) \n было снято с {Profile.FullName}";
                            }
                            return new BaseResponse<IEnumerable<Products>>()
                            {
                                Description = description,
                                StatusCode = StatusCode.Ok,
                                Data = productsToUnbind
                            };
                        }
                    }
                    return new BaseResponse<IEnumerable<Products>>()
                    {
                        Description = $"Количество запрашиваемого товара на снятие меньше или равно 0",
                        StatusCode = StatusCode.UnCreated
                    };
                }

                return new BaseResponse<IEnumerable<Products>>()
                {
                    Description = $"Пользователь не найден",
                    StatusCode = StatusCode.NotFind
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
