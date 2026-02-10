
namespace Application.Enums;

public enum MessageSendStatus
{
    Received = 0,          // Принято от клиента
    SentToProvider = 1,    // Передано провайдеру
    Delivered = 2,         // Доставлено получателю
    ProviderSendingError = 10,   // Ошибка отправки провайдеру
}