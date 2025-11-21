using System.Xml.Serialization;

namespace _1iveowl.Models;

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
