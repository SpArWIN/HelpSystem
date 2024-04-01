using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Report;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class ReportsService :IReportService
    {
        private readonly IBaseRepository<Warehouse> _warehouseRepository;
        private readonly IBaseRepository<Products> _productRepository;
        private readonly IBaseRepository<ProductMovement> _productMovementRepository;

        public ReportsService(IBaseRepository<Products> productRepository,
            IBaseRepository<ProductMovement> productMovementRepository,IBaseRepository<Warehouse> warehouse)
        {
            _productMovementRepository = productMovementRepository;
            _productRepository = productRepository;
            _warehouseRepository = warehouse;
        }
        public async Task<IBaseResponse<ReportsProductOnWarehouseViewModel>> GetWarehouseReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                //Получаем все склады
                var Warehouses = await _warehouseRepository.GetAll()
                    .Select(x => x)
                    .ToListAsync();
                //там  проходимся по каждому складу и смотрим есть ли на нём товар, и так далее.

            }
            catch (Exception e)
            {
              
            }
        }
    }
}
