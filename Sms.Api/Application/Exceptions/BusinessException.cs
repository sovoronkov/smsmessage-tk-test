using System.Text.Json;

namespace Domain.Exceptions;

public class BusinessException : Exception
{
    public object? ResponseObject { get; }

    public BusinessException(object responseObject)
        : base(JsonSerializer.Serialize(responseObject))
    {
        ResponseObject = responseObject;
    }

    public BusinessException(string message) : base(message) { }
}