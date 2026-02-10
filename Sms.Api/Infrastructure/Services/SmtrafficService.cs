using System.Data.Common;
using System.Net.WebSockets;
using System.Text.Json;
using System.Xml.Serialization;
using Application.Dto;
using Application.Enums;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Settings;

public class SmtrafficService : ISmsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SmtrafficService> _logger;
    private readonly SmstrafficSettings _settings;
    private readonly IMemoryCache _cache;
    private readonly object _lock = new();
    private readonly IDataService _dataService;

    private List<ServerState> _servers = new();
    private string _lastWorkingServer = null;

    public SmtrafficService(
        IHttpClientFactory httpClientFactory,
        ILogger<SmtrafficService> logger,
        IOptions<SmstrafficSettings> settings,
        IMemoryCache memoryCache,
        IDataService dataService
        )
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _settings = settings.Value;
        _cache = memoryCache;
        _dataService = dataService;

        // Проверка конфигурации
        _settings.Validate();

        // Инициализация серверов
        InitializeServers();

        // Восстановление состояния из кэша
        LoadServerStateFromCache();

        _logger.LogInformation("SMS service initialized with {ServerCount} servers", _servers.Count);
    }

    private void InitializeServers()
    {
        _servers = _settings.Servers
            .Where(s => s.Enabled)
            .Select(s => new ServerState
            {
                Name = s.Name,
                BaseUrl = s.BaseUrl,
                Endpoint = s.Endpoint,
                Enabled = s.Enabled,
                Order = s.Order
            })
            .OrderBy(s => s.Order)
            .ToList();
    }

    public async Task<ApiResponse<SmsResponse>> SendSmsAsync(SmsRequestDto request, string remoteIp)
    {
        // Определяем порядок серверов для перебора
        var serversToTry = GetServersInTryOrder();
        ApiResponse<SmsResponse> lastResult = null;

        await _dataService.CreateRecords(message: request.Message, request.Phones, remoteIp);

        // Пробуем отправить по очереди через каждый сервер
        foreach (var server in serversToTry)
        {
            var result = await TrySendToServerAsync(server, request.Phones, request.Message);

            // // vso test
            // result.Success = true;  
            // // vso end

            if (result.Success)
            {
                // Успешно физически отправили, но возможны внутренние ошибки в xml code < 0 
                lock (_lock)
                {
                    server.IsAvailable = true;
                    server.FailureCount = 0;
                    server.LastSuccessTime = DateTime.UtcNow;
                    _lastWorkingServer = server.Name;
                    SaveServerStateToCache();
                }

                // // vso test
                //  var smsResponse = new SmsResponse ()  {
                //     Result="OK",
                //     Code=0,
                //     Description="queued 1 message",
                //     MessageInfos= new List<MessageInfo>()
                // };
                // foreach(var phone in request.Phones)
                // {
                //     smsResponse.MessageInfos.Add( new MessageInfo()
                //     {
                //         Phone = phone,
                //         SmsId = "1111111111222222222233333333334444444444"
                //     });
                // }
                // result.Data =  smsResponse;
                //vso end

                await _dataService.UpdateRecords(result.Data, MessageSendStatus.SentToProvider);
                _logger.LogInformation($"SMS sent successfully via {server} response-> Code: {result.Data.Code} Description: {result.Data.Description} Result: {result.Data.Result}");
                return result;
            }
            else
            {
                // Неудача
                lock (_lock)
                {
                    server.FailureCount++;
                    server.LastFailureTime = DateTime.UtcNow;
                    SaveServerStateToCache();
                }

                _logger.LogWarning("Failed to send via {ServerName}: {Error}",
                    server.Name, result.Error);

                lastResult = result;
            }

        }
        // Все серверы не сработали
        var allServerError = "All SMS servers are unavailable";
        _logger.LogError(allServerError);

        var errorResponce = new SmsResponse();
        errorResponce.Result = ResponseCode.ERROR_RESULT;
        errorResponce.Code = ResponseCode.ERROR_CODE_NOT_200;
        errorResponce.Description = allServerError;
        errorResponce.MessageInfos = new List<MessageInfo>();
        await _dataService.UpdateRecords(errorResponce, MessageSendStatus.ProviderSendingError);


        if (lastResult != null)
        {
            lastResult.Success = false;
            lastResult.ServerUsed = allServerError;
            throw new BusinessException(lastResult);
        }
        else
        {
            throw new BusinessException(new ApiResponse<SmsResponse>
            {
                Success = false,
                ServerUsed = allServerError
            });
        }
    }

    private List<ServerState> GetServersInTryOrder()
    {
        var orderedServers = new List<ServerState>();
        var allServers = _servers.OrderByDescending(p => p.LastSuccessTime);
        orderedServers.AddRange(allServers);
        return orderedServers;
    }

    private async Task<ApiResponse<SmsResponse>> TrySendToServerAsync(
        ServerState server,
        IEnumerable<string> phones,
        string message)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(server.Name);

            var formData = new Dictionary<string, string>
            {
                ["login"] = _settings.Login,
                ["password"] = _settings.Password,
                ["message"] = $"your code is {message}",
                ["phones"] = string.Join(",", phones),
                ["rus"] = "1",
                ["want_sms_ids"] = "1"
            };

            using var content = new FormUrlEncodedContent(formData);
            //using var response = await client.PostAsync("aasasasasasas", content);
            using var response = await client.PostAsync(server.Endpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("SMS sending via {ServerName} failed. Status: {StatusCode}",
                    server.Name, response.StatusCode);

                return new ApiResponse<SmsResponse>
                {
                    Success = false,
                    Error = $"HTTP error: {response.StatusCode}",
                    ServerUsed = server.Name
                };
            }

            // Чтение и парсингш ответа
            var responseXml = await response.Content.ReadAsStringAsync();
            var smsResponse = ParseXmlResponse(responseXml);

            return new ApiResponse<SmsResponse>
            {
                //Success = smsResponse.Code == 0,
                Success = true,
                Data = smsResponse,
                Error = smsResponse.Code != 0 ? smsResponse.Description : null,
                ServerUsed = server.Name
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request error when sending SMS via {ServerName}", server.Name);
            return new ApiResponse<SmsResponse>
            {
                Success = false,
                Error = $"Network error: {ex.Message}",
                ServerUsed = server.Name
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when sending SMS via {ServerName}", server.Name);
            return new ApiResponse<SmsResponse>
            {
                Success = false,
                Error = ex.Message,
                ServerUsed = server.Name
            };
        }
    }

    public async Task<List<ServerState>> GetServerStatusAsync()
    {
        return await Task.FromResult(_servers);
    }

    public string GetCurrentStatus()
    {
        var available = _servers.Count(s => s.IsAvailable && !s.IsCooldown);
        var total = _servers.Count;
        var lastWorking = _lastWorkingServer ?? "none";

        return $"Servers: {available}/{total} available, Last working: {lastWorking}";
    }

    private void LoadServerStateFromCache()
    {
        var cachedServers = _cache.Get<List<ServerState>>("Smstraffic_Servers_State");
        var cachedLastWorking = _cache.Get<string>("Smstraffic_Last_Working");

        if (cachedServers != null)
        {
            // Обновляем состояния существующих серверов
            foreach (var cachedServer in cachedServers)
            {
                var currentServer = _servers.FirstOrDefault(s => s.Name == cachedServer.Name);
                if (currentServer != null)
                {
                    currentServer.IsAvailable = cachedServer.IsAvailable;
                    currentServer.FailureCount = cachedServer.FailureCount;
                    currentServer.LastFailureTime = cachedServer.LastFailureTime;
                    currentServer.LastSuccessTime = cachedServer.LastSuccessTime;
                }
            }
        }

        if (!string.IsNullOrEmpty(cachedLastWorking))
        {
            _lastWorkingServer = cachedLastWorking;
        }
    }

    private void SaveServerStateToCache()
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromHours(1)
        };

        _cache.Set("Smstraffic_Servers_State", _servers, cacheOptions);
        _cache.Set("Smstraffic_Last_Working", _lastWorkingServer, cacheOptions);
    }

    private SmsResponse ParseXmlResponse(string xml)
    {
        try
        {
            using var reader = new StringReader(xml);
            var serializer = new XmlSerializer(typeof(SmsResponseProviderDto));

            if (serializer.Deserialize(reader) is not SmsResponseProviderDto dto)
                throw new InvalidOperationException("Failed to deserialize XML response");

            return new SmsResponse
            {
                Result = dto.Result,
                Code = dto.Code,
                Description = dto.Description,
                MessageInfos = dto.MessageInfos?.MessageInfo?.Select(mi => new MessageInfo
                {
                    Phone = mi.Phone,
                    SmsId = mi.SmsId
                }).ToList() ?? new List<MessageInfo>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing XML response");
            throw new InvalidOperationException("Failed to parse SMS response", ex);
        }
    }


}
