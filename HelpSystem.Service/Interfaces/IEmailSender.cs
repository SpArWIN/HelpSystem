using HelpSystem.Domain.Response;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Service.Interfaces
{
    public interface IEmailSender
    {
        Task<BaseResponse<MailMessage>> RecoveryPassword(string Email);
    }
}
