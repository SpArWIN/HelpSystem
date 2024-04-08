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
using HelpSystem.Domain.ViewModel.Report;
using HelpSystem.Domain.ViewModel.Users;
using HelpSystem.Domain.ViewModel.Warehouse;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class ReportsService : IReportService
    {
        private readonly IBaseRepository<Warehouse> _warehouseRepository;
        private readonly IBaseRepository<Products> _productRepository;
        private readonly IBaseRepository<ProductMovement> _productMovementRepository;
        private readonly IBaseRepository<Invoice> _invoiceRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Profile> _profileRepository;

        public ReportsService(IBaseRepository<Products> productRepository, IBaseRepository<User> users,
            IBaseRepository<Profile> profile,
            IBaseRepository<ProductMovement> productMovementRepository, IBaseRepository<Warehouse> warehouse,
            IBaseRepository<Invoice> invoice)
        {
            _productMovementRepository = productMovementRepository;
            _productRepository = productRepository;
            _warehouseRepository = warehouse;
            _invoiceRepository = invoice;
            _profileRepository = profile;
            _userRepository = users;
        }

        public async Task<IBaseResponse<ReportsProductOnWarehouseViewModel>> GetWarehouseReport(DateTime startDate,
            DateTime endDate)
        {
            try
            {
                //Получаем все склады
                var Warehouses = await _warehouseRepository.GetAll()
                    .Include(p => p.Products)
                    .ToListAsync();

                var ReportData = new ReportsProductOnWarehouseViewModel();
                ReportData.StartTime = startDate.ToString("d");
                ReportData.EndTime = endDate.ToString("d");


                DateTime StartDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                DateTime EndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                //Теперь проходимся по всем складам

                foreach (var wh in Warehouses)
                {
                    var productsOnWarehouse = new List<Products>();
                    //Записи о товарах, которые пришли на склад 
                    var incomingMovements = await _productMovementRepository.GetAll()
                        .Where(x => x.DestinationWarehouseId == wh.Id && x.MovementDate >= StartDate &&
                                    x.MovementDate <= EndDate)
                        .OrderByDescending(x => x.MovementDate)
                        .ToListAsync();
                    //Сами товары
                    var latestIncomingProducts = incomingMovements.Select(x => x.Product).ToList();
                    //Записи о товарах которые ушли со склада
                    var outgoingMovements = await _productMovementRepository.GetAll()
                        .Where(x => x.SourceWarehouseId == wh.Id && x.MovementDate >= StartDate &&
                                    x.MovementDate <= EndDate)
                        .OrderByDescending(x => x.MovementDate)
                        .ToListAsync();
                    //Сами ушедшие товары
                    var latestOutgoingProducts = outgoingMovements.Select(x => x.Product).ToList();
                    //ТЕперь смотрим по накладным
                    var invoicesOnWarehouse = await _invoiceRepository.GetAll()
                        .Where(x => x.Products.Any(p => p.Warehouse == wh) && x.CreationDate >= StartDate &&
                                    x.CreationDate <= EndDate)
                        .ToListAsync();
                    foreach (var invoice in invoicesOnWarehouse)
                    {
                        productsOnWarehouse.AddRange(invoice.Products);
                    }
                    //Проходимся циклом по тем товарам которые пришли и смотрим, нет ли в них ушедших

                    foreach (var incomingProduct in latestIncomingProducts)
                    {
                        if (latestIncomingProducts.Any(x => x.Id == incomingProduct.Id))
                        {
                            productsOnWarehouse.Add(incomingProduct);

                        }
                    }

                    foreach (var outgoingProduct in latestOutgoingProducts)
                    {
                        if (latestOutgoingProducts.Any(x => x.Id == outgoingProduct.Id))
                        {
                            productsOnWarehouse.Remove(outgoingProduct);
                        }
                    }

                    //Рассчитываем перемещённые товары
                    //var movedProducts = outgoingMovements.Select(x => x.Product).Distinct().Count();
                    var ProductInfo = productsOnWarehouse
                        .GroupBy(p => p.NameProduct)
                        .Select(x => new ProductsInfo()
                        {
                            ProductName = x.Key,
                            InventoryCode = x.First().InventoryCode,
                            QuantityOnWarehouse = x.Count(),
                            AvailableQuantity = x.Count(x => x.UserId == null),
                        }).ToList();

                    //Берем все количество 
                    var TotalQuantity = ProductInfo.Sum(p => p.QuantityOnWarehouse);

                    var WarehouseReport = new WarehouseReports()
                    {
                        WarehouseName = wh.Name,
                        ProductsInfo = ProductInfo,
                        TotalQuantity = TotalQuantity,
                    };
                    ReportData.WarehousesReports.Add(WarehouseReport);

                }

                return new BaseResponse<ReportsProductOnWarehouseViewModel>()
                {
                    Data = ReportData,
                    Description =
                        $"Отчёт по товарам за \n {startDate.ToShortDateString()} по {endDate.ToShortDateString()} сформирован",
                    StatusCode = StatusCode.Ok
                };

            }
            catch (Exception e)
            {
                return new BaseResponse<ReportsProductOnWarehouseViewModel>()
                {
                    Description = $"{e.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<UserReportViewModel>>> GetUserReports(Guid userid)
        {
            try
            {
                var products = await _productRepository.GetAll()
                    .Where(x => x.UserId == userid)
                    .ToListAsync();

                var Profile = await _profileRepository.GetAll()
                    .Where(x => x.UserId == userid)
                    .FirstOrDefaultAsync();


                var groupedProducts = products
                    .GroupBy(p => new { p.NameProduct, p.InventoryCode })
                    .Select(g => new UserReportViewModel
                    {
                        Name = Profile.Name,
                        LastName = Profile.LastName,
                        SurName = Profile.Surname,
                        FullName = $"{Profile.LastName} {Profile.Name} {Profile.Surname}",
                        ProductName = g.Key.NameProduct,
                        Code = g.Key.InventoryCode,
                        Quantity = g.Count(),

                    })
                    .ToList();
                var TotalQuantity = groupedProducts.Sum(x => x.Quantity);

                foreach (var total in groupedProducts)
                {
                    total.TotalCount = TotalQuantity;
                }

                if (groupedProducts.Any())
                {


                    return new BaseResponse<IEnumerable<UserReportViewModel>>()
                    {
                        Data = groupedProducts,
                        StatusCode = StatusCode.Ok,
                        Description =
                            $"Отчёт по пользователю {Profile.LastName} {Profile.Name} {Profile.Surname} успешно сформирован"
                    };
                }

                return new BaseResponse<IEnumerable<UserReportViewModel>>()
                {
                    Data = Enumerable.Empty<UserReportViewModel>(),
                    StatusCode = StatusCode.Ok,
                    Description = $"{Profile.LastName} {Profile.Name} {Profile.Surname}, не имеет закреплённых товаров."
                };



            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<UserReportViewModel>>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<ReportUSERSViewModel>> GetUsersReports()
        {
            try
            {

                var Users = await _userRepository.GetAll()
                    .Include(p => p.Profile)
                    .Where(x => x.RoleId != 3)
                    .ToListAsync();
                var ReportData = new ReportUSERSViewModel();


                foreach (var user in Users)
                {
                    var Products = await _productRepository.GetAll()
                        .Where(x => x.UserId == user.Id)
                        .ToListAsync();

                    if (Products.Any())
                    {


                        //Группируем товары по наименованию и инвентарному коду
                        var GroupedProducts = Products
                            .GroupBy(p => new { p.NameProduct, p.InventoryCode })
                            .Select(group => new BindingProductViewModel()
                            {
                                NameProduct = group.Key.NameProduct,
                                InventoryCod = group.Key.InventoryCode,
                                TotalCount = group.Count()
                            }).ToList();
                        //Подсчитываем общее количество прикреплённых товаров у юзверя
                        int TotalCount = GroupedProducts.Sum(x => x.TotalCount);

                        var UserInfoViewModel = new UserInfoViewModel()
                        {
                            Login = user.Login,
                            Name = user.Profile.Name,
                            LastName = user.Profile.LastName,
                            SurName = user.Profile.Surname,
                            TotalProducts = TotalCount,
                            UserProducts = GroupedProducts
                        };
                        ReportData.Users.Add(UserInfoViewModel);
                     

                    }
                    else
                    {
                        ReportData.Users.Add(new UserInfoViewModel()
                        {
                            Login = user.Login,
                            Name = user.Profile.Name,
                            LastName = user.Profile.LastName,
                            SurName = user.Profile.Surname,
                            TotalProducts = 0, // Устанавливаем общее количество товаров в 0
                            UserProducts = new List<BindingProductViewModel>(), // Пустой список товаров

                            Message = $"У пользователя {user.Profile.LastName} {user.Profile.Name} {user.Profile.Surname} нет закрепленных товаров."
                        });
                    }
                    
                
                    
                   

                   
                }
                return new BaseResponse<ReportUSERSViewModel>()
                {
                    Data = ReportData,
                    Description = $"Отчёт по всем пользователям успешно сформирован",
                    StatusCode = StatusCode.Ok
                };

            }
            catch (Exception ex)
            {
                return new BaseResponse<ReportUSERSViewModel>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

    }
}
