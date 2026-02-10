namespace Application.Dto;

public class SmsResponse
{
    public string Result { get; set; }
    public int Code { get; set; }
    public string Description { get; set; }
    public List<MessageInfo>? MessageInfos { get; set; }
}
