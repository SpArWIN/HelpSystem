using System.Security.Cryptography.X509Certificates;
using HelpSystem.DAL.Implementantions;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Product;
using HelpSystem.Domain.ViewModel.Product.ProductAllInfo;
using HelpSystem.Domain.ViewModel.Transfer;
using HelpSystem.Domain.ViewModel.Users;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HelpSystem.Service.Implementantions
{
    public class ProductService : IProductService
    {
        private readonly IBaseRepository<Invoice> _invoiceRepository;
        private readonly IBaseRepository<Products> _productsRepository;
        private readonly IBaseRepository<Warehouse> _warehouseRepository;
        private readonly IBaseRepository<Provider> _providerRepository;
        private readonly IBaseRepository<Statement> _statementRepository;
        private readonly IBaseRepository<ProductMovement> _productMovementRepository;
        public ProductService(IBaseRepository<Products> productsRepository, IBaseRepository<Invoice> invoiceRepository, IBaseRepository<Warehouse> waRepository, IBaseRepository<Provider> provideRepository, IBaseRepository<Statement> statement, IBaseRepository<ProductMovement> productMovementRepository)
        {

            _productsRepository = productsRepository;
            _warehouseRepository = waRepository;
            _providerRepository = provideRepository;
            _invoiceRepository = invoiceRepository;
            _statementRepository = statement;
            _productMovementRepository = productMovementRepository;
        }

        /// <summary>
        /// Метод получения товара по его имени или артикулу
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public async Task<BaseResponse<Dictionary<int, string>>> GetProduct(string term)
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
                    var productsLocationInfo = new Dictionary<int, string>();
                    // Обрабатываем каждый товар
                    foreach (var product in productsInMemory)
                    {
                        // Находим последнее перемещение товара
                        var lastMovement = await _productMovementRepository.GetAll()
                            .OrderByDescending(pm => pm.MovementDate)
                            .FirstOrDefaultAsync(pm => pm.ProductId == product.Id);

                        if (lastMovement != null)
                        {
                            // Получаем наименование склада назначения из последнего перемещения
                            var destinationWarehouseName = await _warehouseRepository.GetAll()
                                .Where(x => x.Id == lastMovement.DestinationWarehouseId)
                                .Select(x => x.Name)
                                .FirstOrDefaultAsync();

                            // Формируем строку с информацией о последнем перемещении товара
                            var productLocationInfo = $"{product.NameProduct} ({product.InventoryCode}) СКЛАД: {destinationWarehouseName}";

                            // Добавляем информацию о последнем перемещении товара в словарь
                            productsLocationInfo[product.Id] = productLocationInfo;
                        }
                        else
                        {
                            // Если перемещений нет, формируем информацию о товаре на его первоначальном складе
                            var initialWarehouseName = product.Warehouse.Name;
                            var initialProductInfo = $"{product.NameProduct} ({product.InventoryCode}) СКЛАД: {initialWarehouseName}";

                            // Добавляем информацию о товаре на первоначальном складе в словарь
                            productsLocationInfo[product.Id] = initialProductInfo;
                        }
                    }

                    return new BaseResponse<Dictionary<int, string>>()
                    {
                        StatusCode = StatusCode.Ok,
                        Data = productsLocationInfo
                    };



                }
                else // Если товары не найдены, вернем специальное значение
                {
                    return new BaseResponse<Dictionary<int, string>>()
                    {
                        StatusCode = StatusCode.NotFind,
                        Description = "Товары не найдены"
                    };
                }

            }
            catch (Exception ex)
            {
                return new BaseResponse<Dictionary<int, string>>()
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

                        //Для уникальности товаров, для их идентификационных номеров будем добавлять id
                        //Найдем максимальный id 
                        
                        for (int i = 0; i < quantity; i++)
                        {
                            var NewProduct = new Products
                            {
                                InventoryCode = $"{pos.InventoryCode}",
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
                    // После добавления всех продуктов обновляем InventoryCode
                 

                    foreach (var prod in productsList)
                    {
                       
                        prod.InventoryCode = $"{prod.InventoryCode}_{prod.Id}";
                        await _productsRepository.Update(prod);

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

        public async Task<BaseResponse<Products>> BindingProduct( Guid StatId ,int ProductId,string? Comments)
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

        public async Task<BaseResponse<IEnumerable<Products>>> UnBindingProduct(List<UnbindingProductViewModel> product , Guid ProfileId)
        {
            try
            {
                //Не пустой ли список
                if (!product.Any())
                {
                    return new BaseResponse<IEnumerable<Products>>()
                    {
                        Description = "Список товаров для отвязки пустой",
                        StatusCode = StatusCode.NotFind
                    };
                }

                //Если количество товаров меньше 1, то есть отвязывается один товар
                if (product.Count <= 1)
                {
                    //Находим профиль
                    var Profile = await _productsRepository
                        .GetAll()
                        .Where(x => x.UserId == ProfileId)
                        .Select(x => new
                        {
                            FullName = x.User.Profile.LastName + " " + x.User.Profile.Name + " " +
                                       x.User.Profile.Surname
                        })
                        .FirstOrDefaultAsync();
                    // Список для хранения информации о снятых товарах
                    List<string> removedProductsInfo = new List<string>();
                    if (Profile != null)
                    {
                        foreach (var prod in product)
                        {
                            var ProductToUnbind = await _productsRepository.GetAll()
                                .FirstOrDefaultAsync(x => x.Id == prod.ProductId);

                            ProductToUnbind.UserId = null;
                            ProductToUnbind.Comments = $"Последний владелец: {Profile.FullName}";
                            await _productsRepository.Update(ProductToUnbind);
                            string productInfo = $"{ProductToUnbind.NameProduct} ({ProductToUnbind.InventoryCode})";
                            removedProductsInfo.Add(productInfo);
                        }

                        return new BaseResponse<IEnumerable<Products>>()
                        {
                            Description = $"С {Profile.FullName} снят {string.Join(", ", removedProductsInfo)}",
                            StatusCode = StatusCode.Ok
                        };
                    }

                    return new BaseResponse<IEnumerable<Products>>()
                    {
                        Description = $"Не удалось распознать профиль",
                        StatusCode = StatusCode.NotFind
                    };

                }

                var UserProfule = await _productsRepository
                    .GetAll()
                    .Where(x => x.UserId == ProfileId)
                    .Select(x => new
                    {
                        FullName = x.User.Profile.LastName + " " + x.User.Profile.Name + " " + x.User.Profile.Surname
                    })
                    .FirstOrDefaultAsync();
                // Список для хранения информации о снятых товарах
                List<string> RemovedAllProducts = new List<string>();
                if (UserProfule != null)
                {
                    foreach (var prod in product)
                    {
                        var ProductToUnbind = await _productsRepository.GetAll()
                            .FirstOrDefaultAsync(x => x.Id == prod.ProductId);

                        ProductToUnbind.UserId = null;
                        ProductToUnbind.Comments = $"Последний владелец: {UserProfule.FullName}";
                        await _productsRepository.Update(ProductToUnbind);
                        RemovedAllProducts.Add($"{ProductToUnbind.NameProduct} ({ProductToUnbind.InventoryCode})");
                    }

                    return new BaseResponse<IEnumerable<Products>>()
                    {
                        Description = $"С {UserProfule.FullName} снят {RemovedAllProducts}",
                        StatusCode = StatusCode.Ok
                    };

                }

                return new BaseResponse<IEnumerable<Products>>()
                {
                    Description = $"Не удалось распознать профиль",
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

        public async Task<IBaseResponse<MainProductInfo>> GetMainProductInfo(int id)
        {
            try
            {
                //ищем товар и  работаем
                var Product = await _productsRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (Product != null)
                {
                    var InvoiceProduct = await _invoiceRepository.GetAll()
                        .Where(x => x.Products.Contains(Product))
                        .Select(x => new
                        {
                            x.CreationDate,
                            x.NumberDocument
                        }).FirstOrDefaultAsync();
                    // смотрим по последним перемещениям товара
                    var LastMovements = await _productMovementRepository.GetAll()
                        .OrderByDescending(x => x.MovementDate)
                        .Where(x => x.Product == Product)
                        .FirstOrDefaultAsync();
                    if (LastMovements != null)
                    {
                        //Во первых мы точно теперь знаем его местоположение на текущий момент
                        //Получим все перемещения этого товара
                        var AllMovement = await _productMovementRepository.GetAll()
                            .Where(x => x.Product == Product)
                            .ToListAsync();

                       //Получаем наименование последнего местоположения товара, склад
                       var WarehouseName = await _warehouseRepository.GetAll()
                           .Where(x => x.Id == LastMovements.DestinationWarehouseId)
                           .FirstOrDefaultAsync();
                           


                        //Теперь найдём есть ли у товара привязка 
                        var FindUser = await _productsRepository.GetAll()
                            .Include(u=>u.User)
                            .ThenInclude(p=>p.Profile)
                            .Where(x=>x.Id == id && x.UserId != null)
                            .Select( x=> new UsersViewModel()
                            {
                                Login = x.User.Login,
                                Name = x.User.Profile.Name,
                                LastName = x.User.Profile.LastName,
                                Surname = x.User.Profile.Surname
                            } ).
                            FirstOrDefaultAsync();
                        if (FindUser == null)
                        {
                            //Список всех перемещений
                            List<TransferFindInfo> allTransfersProducts = new List<TransferFindInfo>();

                            //Проходимся по всем перемещениям товара 
                            foreach (var movement in AllMovement)
                            {
                                //Получаем название склада с которого товар ушел
                                var SourceWarehouse = await _warehouseRepository.GetAll()
                                    .FirstOrDefaultAsync(x => x.Id == movement.SourceWarehouseId);

                                var SourceWarehouseName = SourceWarehouse.Name ?? "Склад отправителя не найден";
                                // Получаем название склада, на который товар пришел
                                var DestinationWarehouse = await _warehouseRepository.GetAll()
                                    .FirstOrDefaultAsync(x => x.Id == movement.DestinationWarehouseId);
                                var DestinationWarehouseName = DestinationWarehouse.Name ??"Склад получателя не найден";

                                var transInfo = new TransferFindInfo
                                {
                                    DateTimeIncoming = movement.MovementDate.ToShortTimeString(),
                                    DateTimeOutgoing = movement.MovementDate.ToShortTimeString(),
                                    SourceWarehouseName = SourceWarehouseName,
                                    DestinationWarehouseName = DestinationWarehouseName
                                };
                                allTransfersProducts.Add(transInfo);
                            }



                            //В общем теперь собираем все во едино

                            var AllInfoOut = new MainProductInfo
                            {
                                NameProduct = Product.NameProduct,
                                InventoryCode = Product.InventoryCode,
                                Comments = Product?.Comments,
                                OriginalWarehouse = Product.Warehouse.Name,
                                CurrentWarehouseName = WarehouseName?.Name,
                                Usver = FindUser,
                                AllTransfersProducts = allTransfersProducts,

                            };
                            return new BaseResponse<MainProductInfo>()
                            {
                                Data = AllInfoOut,
                                Description =
                                    $"Вся необходимая информация о {Product.NameProduct} {Product.InventoryCode} была найдена.",
                                StatusCode = StatusCode.Ok
                            };

                        }
                           
                    }
                }

                return new BaseResponse<MainProductInfo>()
                {
                    Description = "Не удалось найти товар",
                    StatusCode = StatusCode.NotFind
                };
            }
            catch (Exception ex)
            {
                 
            }
        }
    }
}
