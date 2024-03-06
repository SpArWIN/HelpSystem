using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.DAL;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;

using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class InvoiceService : IInvoiceService
    {
      private readonly IBaseRepository<Invoice> _invoiceRepository;


      public InvoiceService(IBaseRepository<Invoice> invoiceRepository)
      {
          _invoiceRepository = invoiceRepository;
      }
        public async Task<IBaseResponse<Invoice>> CreateInvoice(string NumberDocument)
        {
            try
            {
                var Response = await _invoiceRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.NumberDocument ==NumberDocument);
                if (Response != null)
                {
                    return new BaseResponse<Invoice>()
                    {
                        Description = "Накладная с таким номером уже существует",
                        StatusCode = StatusCode.UnCreated
                    };
                }

                var Invoice = new Invoice()
                {
                    CreationDate = DateTime.Now,
                    NumberDocument = NumberDocument,

                };
                await _invoiceRepository.Create(Invoice);
                return new BaseResponse<Invoice>()
                {
                    Data = Invoice,
                    Description = $"Товарная накладная с номером {NumberDocument} " +
                                  $"успешно создана ",
                    StatusCode = StatusCode.Ok
                };

            }
            catch (Exception e)
            {
                return new BaseResponse<Invoice>()
                {
                    Description = $"{e.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
