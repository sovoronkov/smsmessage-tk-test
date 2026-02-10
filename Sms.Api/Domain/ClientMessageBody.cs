
using Domain.Interfaces;

namespace Domain.Entities;

public class ClientMessageBody
{
    public long Id { get; private set; }
    public long ClientMessageHeaderId { get; private set; }
    public ClientMessageHeader ClientMessageHeader { get; private set; }
    public string Phone { get; private set; } = string.Empty;
    public string? SmsId { get; private set; } = string.Empty;

    private ClientMessageBody() { } // Для EF Core
    public ClientMessageBody(int clientMessageHeaderId, string phone)
    {
        Phone = phone;
        ClientMessageHeaderId = clientMessageHeaderId;
    }
    public void UpdateSmsId(string smsId)
    {
        SmsId = smsId;
    }

    public void UpdatePhone(string phone)
    {
        Phone = phone;
    }
}

