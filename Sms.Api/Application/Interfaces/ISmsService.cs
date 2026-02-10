
using Application.Dto;

namespace Application.Interfaces;

public interface ISmsService
{
    Task<ApiResponse<SmsResponse>> SendSmsAsync(SmsRequestDto request, string remoteIp);
    Task<List<ServerState>> GetServerStatusAsync();
    string GetCurrentStatus();
}