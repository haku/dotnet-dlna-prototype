using System.Xml.Serialization;

namespace _1iveowl.Models;

[XmlRoot(ElementName = "propertyset", Namespace = "urn:schemas-upnp-org:event-1-0")]
public class PropertySet
{
    [XmlElement(ElementName = "property", Namespace = "urn:schemas-upnp-org:event-1-0")]
    public Property Property { get; set; }

}

public class Property
{
    [XmlElement(Namespace = "")] public string LastChange;
}

[XmlRoot(Namespace = "urn:schemas-upnp-org:metadata-1-0/AVT/")]
public class Event 
{
    public InstanceID InstanceID { get; set; }
}

public class EventEntry
{
    [XmlAttribute(Namespace = "")]
    public string val { get; set; }
}

public class InstanceID : EventEntry
{
    public AVTransportURI              AVTransportURI { get; set; }
    public AVTransportURIMetaData      AVTransportURIMetaData { get; set; }
    public CurrentMediaDuration        CurrentMediaDuration { get; set; }
    public CurrentPlayMode             CurrentPlayMode { get; set; }
    public CurrentTrack                CurrentTrack { get; set; }
    public CurrentTrackDuration        CurrentTrackDuration { get; set; }
    public CurrentTrackMetaData        CurrentTrackMetaData { get; set; }
    public CurrentTrackURI             CurrentTrackURI { get; set; }
    public CurrentTransportActions     CurrentTransportActions { get; set; }
    public TransportPlaySpeed          TransportPlaySpeed { get; set; }
    public TransportState              TransportState { get; set; }
    public TransportStatus             TransportStatus { get; set; }
}

public class AVTransportURI : EventEntry;
public class AVTransportURIMetaData : EventEntry;
public class CurrentMediaDuration : EventEntry;
public class CurrentPlayMode : EventEntry;
public class CurrentTrack : EventEntry;
public class CurrentTrackDuration : EventEntry;
public class CurrentTrackMetaData : EventEntry;
public class CurrentTrackURI : EventEntry;
public class CurrentTransportActions : EventEntry;
public class TransportPlaySpeed : EventEntry;
public class TransportState : EventEntry;
public class TransportStatus : EventEntry;