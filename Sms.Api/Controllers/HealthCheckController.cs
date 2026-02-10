using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Getway.Controllers.System;

/// <summary>
/// Проверка состояния сервиса
/// </summary>
[ApiController]
[Route("api/health-check/")]
[AllowAnonymous]
public class HealthCheckController(ILogger<HealthCheckController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<HealthCheckResult> Check()
    {
        logger.LogInformation("Health check passed");
        return HealthCheckResult.Healthy($"I'm alive");
    }
}