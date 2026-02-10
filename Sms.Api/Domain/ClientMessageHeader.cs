
using Application.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class ClientMessageHeader
{
    public long Id { get; private set; }
    public DateTime DateCreation { get; private set; }
    public string? RemoteIp { get; private set; }
    public string Message { get; private set; }
    public MessageSendStatus Status { get; private set; }
    public DateTime? DateSentToProvider { get; private set; }


    // Ответ 
    public int? ResponseResult { get; set; } // 0 -OK, 1-ERROR
    public int? ResponseCode { get; set; }
    public string? ResponseDescription { get; set; }
    public string? Response { get; set; }
    public List<ClientMessageBody> ClientMessageBodies = [];


    private ClientMessageHeader() { } // Для EF Core

    public ClientMessageHeader(string message, string? remoteIp = null)
    {
        RemoteIp = remoteIp;
        Message = message;
        Status = MessageSendStatus.Received;
        DateCreation = DateTime.UtcNow;
    }

    public void MarkAsSentToProvider()
    {
        DateSentToProvider = DateTime.UtcNow;
    }

    public void UpdateResponse(int? responseResult, int? responseCode, string? responseDescription, string? response)
    {
        ResponseResult = responseResult;
        ResponseCode = responseCode;
        ResponseDescription = responseDescription;
        Response = response;
    }

    public void UpdateSent(MessageSendStatus status)
    {
        DateSentToProvider = DateTime.UtcNow;
        Status = status;
    }
    public void AddMessageBody(ClientMessageBody messageBody)
    {
        ClientMessageBodies.Add(messageBody);
    }

    public void UpdateRemoteIp(string remoteIp)
    {
        RemoteIp = remoteIp;
    }
}

