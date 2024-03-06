using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Response;


namespace HelpSystem.Service.Interfaces
{
    public interface IInvoiceService
    {
        Task<IBaseResponse<Invoice>> CreateInvoice(string NumberDocument);
        //Добавить потом остальные методы
    }
}
