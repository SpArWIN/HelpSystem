using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSystem.Domain.ViewModel.Token
{
    public class TokenInfo
    {
        //Токен и время жизни
        public string Token { get; set; }
        public TimeSpan ExpirationTime { get; set; }
    }
}
