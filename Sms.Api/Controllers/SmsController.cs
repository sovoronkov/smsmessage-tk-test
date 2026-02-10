using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/sms")]
[Authorize]
public class SmsController : ControllerBase
{
    private readonly ISmsService _smsService;
    private readonly ILogger<SmsController> _logger;
    private readonly IMapper _mapper;

    public SmsController(ISmsService smsService, ILogger<SmsController> logger, IMapper mapper)
    {
        _smsService = smsService;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost("send")]
    [ProducesResponseType(typeof(ApiResponse<SmsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendSms([FromBody] SmsRequestModel request)
    {
        var validationResult = ValidateRequest(request);
        if (!validationResult.IsValid)
            return BadRequest(new
            {
                Success = false,
                Error = validationResult.Errors,
            });

        string remoteIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? string.Empty;
        return Ok(await _smsService.SendSmsAsync(_mapper.Map<SmsRequestDto>(request), remoteIp));
    }

    private (bool IsValid, List<string> Errors) ValidateRequest(SmsRequestModel request)
    {
        var errors = new List<string>();

        if (request == null)
        {
            errors.Add("Request cannot be null");
            return (false, errors);
        }

        if (request.Phones == null || !request.Phones.Any())
            errors.Add("At least one phone number is required");
        else if (request.Phones.Any(p => !IsValidPhoneNumber(p)))
            errors.Add("Invalid phone number format");

        if (string.IsNullOrWhiteSpace(request.Message))
            errors.Add("Message cannot be empty");
        else if (request.Message.Length > 160)
            errors.Add("Message is too long (max 300 characters)");

        return (!errors.Any(), errors);
    }

    private bool IsValidPhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        if (!phone.All(char.IsDigit))
            return false;

        var digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
        return digitsOnly.Length >= 10 && digitsOnly.Length <= 30;
    }
}