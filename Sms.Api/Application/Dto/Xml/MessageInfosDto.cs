using System.Xml.Serialization;

namespace Application.Dto;

public class MessageInfosDto
{
    [XmlElement("message_info")]
    public List<MessageInfoDto> MessageInfo { get; set; }
}