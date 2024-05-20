namespace HelpSystem.Domain.ViewModel.Token
{
    public class TokenInfo
    {
        //Токен и время жизни
        public string Token { get; set; }
        public TimeSpan ExpirationTime { get; set; }
    }
}
