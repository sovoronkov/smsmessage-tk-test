
namespace Application.Dto;

public class SmsRequestDto
{
    public List<string> Phones { get; set; }
    public string Message { get; set; }
}