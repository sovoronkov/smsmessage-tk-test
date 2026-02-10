using System.Xml.Serialization;

public class MessageInfoDto
{
    [XmlElement("phone")]
    public string Phone { get; set; }

    [XmlElement("sms_id")]
    public string SmsId { get; set; }
}