using System.Text.Json;
using Application.Dto;
using Application.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Repositories.Interfaces;

public class DataService : IDataService
{
    public ClientMessageHeader ClientMessageHeader { get; set; }
    private readonly IUnitOfWork _unitOfWork;
    public DataService(IUnitOfWork unitOfWork
    )
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ClientMessageHeader?> CreateRecords(string message, List<string> phones, string remoteIp)
    {
        ClientMessageHeader = new ClientMessageHeader(message: message, remoteIp: remoteIp);
        foreach (var phone in phones)
        {
            ClientMessageHeader.AddMessageBody(new ClientMessageBody(clientMessageHeaderId: 0,
                                                                     phone: phone));
        }
        _unitOfWork.ClientMessageHeaders.Insert(ClientMessageHeader);
        await _unitOfWork.ClientMessageHeaders.SaveChangesAsync();
        return ClientMessageHeader;
    }

    public async Task<bool> UpdateRecords(SmsResponse smsResponse, MessageSendStatus status)
    {

        int result = ResponseCode.UNKNOW_CODE;
        if (smsResponse.Result.Equals("OK")) result = ResponseCode.OK_CODE;
        else
          if (smsResponse.Result.Equals("ERROR")) result = ResponseCode.ERROR_CODE;

        ClientMessageHeader.UpdateSent(status);
        ClientMessageHeader.UpdateResponse(responseResult: result,
                                          responseCode: smsResponse.Code,
                                          responseDescription: smsResponse.Description,
                                          JsonSerializer.Serialize(smsResponse));

        foreach (var body in ClientMessageHeader.ClientMessageBodies)
        {
            if (smsResponse.MessageInfos != null)
            {
                var phoneRecord = smsResponse.MessageInfos.Where(p => p.Phone.Equals(body.Phone)).FirstOrDefault();
                if (phoneRecord != null)
                {
                    body.UpdateSmsId(phoneRecord.SmsId);
                }
            }
        }
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
