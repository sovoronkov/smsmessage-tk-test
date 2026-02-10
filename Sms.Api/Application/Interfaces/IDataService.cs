using Application.Dto;
using Application.Enums;
using Domain.Entities;

public interface IDataService
{
    public ClientMessageHeader ClientMessageHeader { get; set; }
    public Task<ClientMessageHeader> CreateRecords(string message, List<string> phones, string remoteIp);
    public Task<bool> UpdateRecords(SmsResponse smsResponse, MessageSendStatus status);
}
