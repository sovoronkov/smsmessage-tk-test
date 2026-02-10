using System.Xml.Serialization;

namespace Application.Dto;


[XmlRoot("reply")]
public class SmsResponseProviderDto
{
    [XmlElement("result")]
    public string Result { get; set; }

    [XmlElement("code")]
    public int Code { get; set; }

    [XmlElement("description")]
    public string Description { get; set; }

    [XmlElement("message_infos")]
    public MessageInfosDto MessageInfos { get; set; }
}