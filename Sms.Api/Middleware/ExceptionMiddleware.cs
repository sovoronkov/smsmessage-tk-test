
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Application.Dto;
using Domain.Exceptions;

namespace Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private const string CONTENT_TYPE = "application/json";
    public ExceptionMiddleware(RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task Invoke(HttpContext context)
    {
        try
        {
            context.Request.EnableBuffering();
            using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                string body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

            }

            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task SetResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = CONTENT_TYPE;
        var json = JsonSerializer.Serialize(new { message },
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {

        if (ex is BusinessException businessEx)
        {
            context.Response.ContentType = CONTENT_TYPE;
            context.Response.StatusCode = StatusCodes.Status502BadGateway;
            return context.Response.WriteAsync(JsonSerializer.Serialize(businessEx.ResponseObject));
        }

        var statusCode = ex switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            HttpRequestException httpEx when httpEx.Message.Contains("401") => StatusCodes.Status401Unauthorized,
            HttpRequestException => StatusCodes.Status502BadGateway,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ApiResponse<SmsResponse>
        {
            Success = false,
            Error = ex.InnerException != null && ex.InnerException.Message != null
                   ? ex.InnerException.Message
                   : ex.Message,

        };

        _logger.LogError(ex, "Exception caught: {Type} at {Path}", ex.GetType().Name, context.Request.Path);

        context.Response.ContentType = CONTENT_TYPE;
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}