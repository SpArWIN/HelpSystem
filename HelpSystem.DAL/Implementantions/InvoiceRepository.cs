using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;

namespace HelpSystem.DAL.Implementantions
{
    public class InvoiceRepository : IBaseRepository<Invoice>
    {
        private readonly AppDbContext _appDbContext;
        public InvoiceRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Create(Invoice entity)
        {
            await _appDbContext.Invoices.AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public IQueryable<Invoice> GetAll()
        {
            return _appDbContext.Invoices;
        }

        public async Task<Invoice> Update(Invoice entity)
        {
            _appDbContext.Invoices.Update(entity);
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Invoice entity)
        {
            _appDbContext.Invoices.Remove(entity);
            await _appDbContext.SaveChangesAsync();


        }


    }
}
