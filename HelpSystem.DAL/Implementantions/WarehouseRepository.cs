using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;

namespace HelpSystem.DAL.Implementantions
{
    public class WarehouseRepository : IBaseRepository<Warehouse>
    {
        private readonly AppDbContext _appDbContext;
        public WarehouseRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(Warehouse entity)
        {
            await _appDbContext.AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public IQueryable<Warehouse> GetAll()
        {
            return _appDbContext.Warehouses;
        }

        public async Task<Warehouse> Update(Warehouse entity)
        {
            _appDbContext.Warehouses.Update(entity);
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Warehouse entity)
        {
            _appDbContext.Warehouses.Remove(entity);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
