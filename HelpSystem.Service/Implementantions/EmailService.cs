using HelpSystem.DAL.Implementantions;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Helpers;
using HelpSystem.Domain.Response;

using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HelpSystem.Service.Implementantions
{
    public class EmailService : IEmailSender
    {
        private readonly string _emailConnect = "HelpDeskSystem@outlook.com";
        protected readonly string pass = "njxyjdctpyftn!21";
		private readonly IBaseRepository<Profile> _profileRepository;
        public EmailService(IBaseRepository<Profile> profile)
		{
			_profileRepository = profile;
		}
		
		
	
        public async Task<BaseResponse<MailMessage>> RecoveryPassword(string Email)
        {
			try
			{
				var UsProfile = await _profileRepository.GetAll()
					.Where(x => x.Email == Email)
					.FirstOrDefaultAsync();
				if(UsProfile == null)
				{
					return await Task.FromResult(new BaseResponse<MailMessage>
					{
						Description = "Пользователь с указанной почтой не найден",
						StatusCode = Domain.Enum.StatusCode.NotFind
					});
				}
				
				var CurrentTime = DateTime.Now;
				string TimeOfDay;
				if(CurrentTime.TimeOfDay < TimeSpan.FromHours(12))
				{
					TimeOfDay = "Доброе утро";
				}
				else if(CurrentTime.TimeOfDay <= TimeSpan.FromHours(18))
				{
					TimeOfDay = "Добрый день";
				}
				else
				{
					TimeOfDay = "Добрый вечер";
				}
				//ДЛя того, чтобы ссылка действовала всего один раз, создадим временный токен
				var TemporyTokey = Guid.NewGuid().ToString();
				var hashedToken = HashPassword.HashPassowrds(TemporyTokey);

                //Формируем ссылку с восстановлением
                var recoveryLink = $"https://localhost:44381/Account/RecoveryPassword?token={hashedToken}&userId={UsProfile.UserId}";
                var htmlBody = $@"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Восстановление пароля</title>
    <style>
        .card {{
            width: 500px;
            margin: 0 auto;
            border: 1px solid #ccc;
            border-radius: 8px;
            padding: 10px;
        }}
        .card-header {{
            text-align: center;
        }}
        .card-body {{
            padding: 10px;
        }}
        .card-footer {{
            text-align: center;
        }}
        .card-button {{
            display: block;
            width: 100%;
            padding: 10px;
            text-align: center;
            background-color: #007bff;
            color: #fff;
            text-decoration: none;
            border-radius: 5px;
            margin-top: 10px;
        }}
    </style>
</head>
<body>
    <div class='card'>
        <div class='card-header'>
            <h3>Служба восстановления пароля HelpDesk</h3>
        </div>
        <div class='card-body'>
            <p>{TimeOfDay}, {UsProfile.LastName} {UsProfile.Name} {UsProfile.Surname}, это письмо с восстановлением пароля из системы HelpDesk.</p>
        </div>
        <div class='card-footer'>
            <a href='{recoveryLink}' class='card-button'>Восстановить пароль</a>
        </div>
    </div>
</body>
</html>";

                var Message = new MailMessage(_emailConnect, Email)
				{
					Subject = "Восстановление пароля",
					Body = htmlBody,
					IsBodyHtml = true
				};


                var Client = new SmtpClient("smtp-mail.outlook.com", 587)
				{
					EnableSsl = true,
					Credentials = new  NetworkCredential(_emailConnect, pass)
				};

				Client.Send(Message);
				return await Task.FromResult(new BaseResponse<MailMessage>
				{
					Data = Message,
					StatusCode = Domain.Enum.StatusCode.Ok,
					Description = $"Сообщение успешно доставлено"
				});

			}
			catch (Exception ex)
			{
				return await Task.FromResult(new BaseResponse<MailMessage>
				{
					Description = ex.Message,
					StatusCode = Domain.Enum.StatusCode.InternalServerError
				});
				
			}
        }
    }
}
