using System.Xml.Serialization;

namespace _1iveowl.Models;

[XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class Envelope
{
    [XmlAttribute(AttributeName = "encodingStyle", Namespace = "")]
    public string EncodingStyle { get; set; } = "http://schemas.xmlsoap.org/soap/encoding/";

    [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public Body Body { get; set; }
}

public class Body
{
    [XmlElement(Namespace = "urn:schemas-upnp-org:service:AVTransport:1")]
    public SetAVTransportURI? SetAVTransportURI { get; set; }

    [XmlElement(Namespace = "urn:schemas-upnp-org:service:AVTransport:1")]
    public Play? Play { get; set; }

    [XmlElement(Namespace = "urn:schemas-upnp-org:service:AVTransport:1")]
    public Pause? Pause { get; set; }

    [XmlElement(Namespace = "urn:schemas-upnp-org:service:AVTransport:1")]
    public Stop? Stop { get; set; }

    [XmlElement(Namespace = "urn:schemas-upnp-org:service:AVTransport:1")]
    public Next? Next { get; set; }

    [XmlElement(Namespace = "urn:schemas-upnp-org:service:AVTransport:1")]
    public Previous? Previous { get; set; }

    [XmlElement(Namespace = "urn:schemas-upnp-org:service:AVTransport:1")]
    public GetTransportInfo? GetTransportInfo { get; set; }

    [XmlElement(Namespace = "urn:schemas-upnp-org:service:AVTransport:1")]
    public GetPositionInfo? GetPositionInfo { get; set; }

    [XmlElement(Namespace = "urn:schemas-upnp-org:service:AVTransport:1")]
    public GetMediaInfo? GetMediaInfo { get; set; }
}

public abstract class Action
{
    [XmlElement(Namespace = "")]
    public string InstanceID { get; set; }
}

public class SetAVTransportURI : Action
{
    [XmlElement(Namespace = "")]
    public string CurrentURI { get; set; }

    [XmlElement(Namespace = "")]
    public string CurrentURIMetaData { get; set; }
}

public class Play : Action
{
    [XmlElement(Namespace = "")]
    public string Speed { get; set; }
}

public class Pause : Action;
public class Stop : Action;
public class Next : Action;
public class Previous : Action;
public class GetTransportInfo : Action;
public class GetPositionInfo : Action;
public class GetMediaInfo : Action;
