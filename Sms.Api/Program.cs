using System.Net;
using Application.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Persistence.Classes;
using Persistence.Repositories;
using Polly;
using Polly.Extensions.Http;
using Repositories.Interfaces;
using Settings;
using Api.Mappings;
using Microsoft.AspNetCore.Authentication;
using Api.Middleware;

const string ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT);
Console.WriteLine($"--- ASPNETCORE_ENVIRONMENT is {environment}");
if (!string.IsNullOrEmpty(environment))
{
    var apps = $"appsettings.{environment}.json";
    builder.Configuration.AddJsonFile(apps, optional: true, reloadOnChange: true);
    Console.WriteLine($"--- AppSetting file is used {apps}");
}

// Конфигурация
builder.Services.Configure<SmstrafficSettings>(
    builder.Configuration.GetSection(SmstrafficSettings.SectionName));

builder.Services.Configure<AuthSettings>(
    builder.Configuration.GetSection(AuthSettings.SectionName));

// Добавляем аутентификацию
builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

// Добавляем авторизацию
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BasicAuthentication", policy =>
        policy.RequireAuthenticatedUser());
});

// MemoryCache
builder.Services.AddMemoryCache();

// Регистрация HTTP-клиентов для каждого сервера
var smsSettings = builder.Configuration.GetSection(SmstrafficSettings.SectionName)
    .Get<SmstrafficSettings>();

if (smsSettings?.Servers != null)
{
    foreach (var server in smsSettings.Servers.Where(s => s.Enabled))
    {
        builder.Services.AddHttpClient(server.Name, (serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<SmstrafficSettings>>().Value;

            client.BaseAddress = new Uri(server.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
            //client.DefaultRequestHeaders.Add("User-Agent", "SmsService/1.0");
            //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
        })
        .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<SmstrafficSettings>>().Value;
            return CreateHttpMessageHandler(settings);
        })
        .AddPolicyHandler((serviceProvider, request) =>
            CreateRetryPolicy(serviceProvider, server.Name));
    }
}

builder.Services.AddDataBase(builder.Configuration);
builder.Services.AddScoped<ISmsService, SmtrafficService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddAutoMapper(typeof(MappingProfileApi));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("basic", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "basic",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Basic Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddHealthChecks();
// Health Checks
// builder.Services.AddHealthChecks()
//     .AddCheck<SmstrafficHealthCheck>("smstraffic");



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();
app.MapHealthChecks("/api/health-check");
app.Run();

// Вспомогательные методы
HttpMessageHandler CreateHttpMessageHandler(SmstrafficSettings settings)
{
    var handler = new HttpClientHandler();

    if (!settings.EnableSslValidation && builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }

    handler.UseProxy = false;
    return handler;
}

IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(IServiceProvider serviceProvider, string serverName)
{
    var settings = serviceProvider.GetRequiredService<IOptions<SmstrafficSettings>>().Value;
    var logger = serviceProvider.GetRequiredService<ILogger<SmtrafficService>>();

    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(
            settings.RetryCount,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                logger.LogWarning(
                    "Retry {RetryAttempt} for server {ServerName} after {Delay}ms",
                    retryAttempt, serverName, timespan.TotalMilliseconds);
            });
}

// Health Check
public class SmstrafficHealthCheck : IHealthCheck
{
    private readonly ISmsService _smsService;

    public SmstrafficHealthCheck(ISmsService smsService)
    {
        _smsService = smsService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var servers = await _smsService.GetServerStatusAsync();
            var availableServers = servers.Count(s => s.IsAvailable && !s.IsCooldown);

            if (availableServers > 0)
            {
                return HealthCheckResult.Healthy(
                    $"{availableServers} SMS servers available");
            }

            return HealthCheckResult.Unhealthy("No SMS servers available");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"SMS service error: {ex.Message}");
        }
    }
}

