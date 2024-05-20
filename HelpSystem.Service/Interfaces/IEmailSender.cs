using HelpSystem.Domain.Response;
using System.Net.Mail;

namespace HelpSystem.Service.Interfaces
{
    public interface IEmailSender
    {
        Task<BaseResponse<MailMessage>> RecoveryPassword(string Email);
    }
}
