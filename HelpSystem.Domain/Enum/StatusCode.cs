namespace HelpSystem.Domain.Enum
{
    public enum StatusCode
    {
        //Не создано
        UnCreated = 8,
        //Отлично
        Ok = 200,
        //Внутренняя ошибка
        InternalServerError = 500,
        //Пользователь уже зарегистрирован
        UserIsRegistered = 5,
        //Не найден, объект
        NotFind = 11,
        //Неизменено
        UnChanched = 12
    }
}
