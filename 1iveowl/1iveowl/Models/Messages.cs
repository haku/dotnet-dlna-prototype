using System.Xml.Serialization;

namespace _1iveowl.Models;

public class GetTransportInfoResponse
{
    [XmlElement(Namespace = "")]
    public string CurrentTransportState { get; set; }

    [XmlElement(Namespace = "")]
    public string CurrentTransportStatus { get; set; }

    [XmlElement(Namespace = "")]
    public string CurrentSpeed { get; set; }
}
