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
using HelpSystem.Domain.ViewModel.Warehouse;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class ReportsService :IReportService
    {
        private readonly IBaseRepository<Warehouse> _warehouseRepository;
        private readonly IBaseRepository<Products> _productRepository;
        private readonly IBaseRepository<ProductMovement> _productMovementRepository;
        private readonly IBaseRepository<Invoice> _invoiceRepository;
        public ReportsService(IBaseRepository<Products> productRepository,
            IBaseRepository<ProductMovement> productMovementRepository,IBaseRepository<Warehouse> warehouse,IBaseRepository<Invoice> invoice)
        {
            _productMovementRepository = productMovementRepository;
            _productRepository = productRepository;
            _warehouseRepository = warehouse;
            _invoiceRepository = invoice;
        }
        public async Task<IBaseResponse<ReportsProductOnWarehouseViewModel>> GetWarehouseReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                //Получаем все склады
                var Warehouses = await _warehouseRepository.GetAll()
                    .Include(p=>p.Products)
                    .ToListAsync();
                var response = new BaseResponse<ReportsProductOnWarehouseViewModel>();
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
                        .Where(x => x.DestinationWarehouseId == wh.Id && x.MovementDate >= StartDate && x.MovementDate <= EndDate)
                        .OrderByDescending(x => x.MovementDate)
                        .ToListAsync();
                    //Сами товары
                    var latestIncomingProducts = incomingMovements.Select(x => x.Product).ToList();
                    //Записи о товарах которые ушли со склада
                    var outgoingMovements = await _productMovementRepository.GetAll()
                        .Where(x => x.SourceWarehouseId == wh.Id && x.MovementDate >= StartDate && x.MovementDate <= EndDate)
                        .OrderByDescending(x => x.MovementDate)
                        .ToListAsync();
                        //Сами ушедшие товары
                    var latestOutgoingProducts = outgoingMovements.Select(x => x.Product).ToList();
                    //ТЕперь смотрим по накладным
                    var invoicesOnWarehouse = await _invoiceRepository.GetAll()
                        .Where(x => x.Products.Any(p => p.Warehouse == wh) && x.CreationDate >= StartDate && x.CreationDate <= EndDate)
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
                    var movedProducts = outgoingMovements.Select(x => x.Product).Distinct().Count();
                    var ProductInfo = productsOnWarehouse
                        .GroupBy(p => p.NameProduct)
                        .Select(x => new ProductsInfo()
                        {
                            ProductName = x.Key,
                            InventoryCode = x.First().InventoryCode,
                            QuantityOnWarehouse = x.Count(),
                            AvailableQuantity = x.Count(x => x.UserId == null),
                            MovedQuantity = movedProducts
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
    }
}
