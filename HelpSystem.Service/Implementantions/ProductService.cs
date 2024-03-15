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
        private readonly IBaseRepository<Statement> _statementRepository;
        public ProductService(IBaseRepository<Products> productsRepository, IBaseRepository<Warehouse> waRepository, IBaseRepository<Provider> provideRepository, IBaseRepository<Statement> statement)
        {

            _productsRepository = productsRepository;
            _warehouseRepository = waRepository;
            _providerRepository = provideRepository;
            //_invoiceRepository = invoiceRepository;
            _statementRepository = statement;
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
                    .ToListAsync(); // Загрузить все товары в память

                if (productsInMemory.Any()) // Проверяем, найдены ли товары
                {
                    var products = productsInMemory
                        .GroupBy(x => x.NameProduct) // Группируем товары по наименованию
                        .Select(group => group.First()) // Берем первый товар из каждой группы 
                        .Select(x => new
                        {
                            x.Id,
                            Name = $"{x.NameProduct} ({x.InventoryCode})",
                            Location = x.Warehouse.Name
                        })
                        .ToDictionary(x => x.Id, x => x.Name);

                    return new BaseResponse<Dictionary<Guid, string>>()
                    {
                        StatusCode = StatusCode.Ok,
                        Data = products
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
                        FullName = x.User.Profile.Surname + " " + x.User.Profile.Name + " " + x.User.Profile.LastName
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
                                await _productsRepository.Update(prod);
                            }

                            return new BaseResponse<IEnumerable<Products>>()
                            {
                                Description = $"Товар(ы) {product.NameProduct} ({product.Code}) \n сняты с \n{Profile.FullName}",
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
