using System.Xml.Serialization;

namespace _1iveowl.Models;

public class GetMediaInfoResponse
{
    [XmlElement(Namespace = "")] public string NrTracks { get; set; } = "0";
    [XmlElement(Namespace = "")] public string CurrentURI { get; set; } = "";
    [XmlElement(Namespace = "")] public string CurrentURIMetaData { get; set; } = "";
    [XmlElement(Namespace = "")] public string MediaDuration { get; set; } = "";
    [XmlElement(Namespace = "")] public string NextURI { get; set; } = "";
    [XmlElement(Namespace = "")] public string NextURIMetaData { get; set; } = "";
    [XmlElement(Namespace = "")] public string PlayMedium { get; set; } = "NONE";
    [XmlElement(Namespace = "")] public string RecordMedium { get; set; } = "NOT_IMPLEMENTED";
    [XmlElement(Namespace = "")] public string WriteStatus { get; set; } = "NOT_IMPLEMENTED";
}

public class GetPositionInfoResponse
{
    [XmlElement(Namespace = "")] public string Track { get; set; } = "0";
    [XmlElement(Namespace = "")] public string TrackDuration { get; set; } = "";
    [XmlElement(Namespace = "")] public string TrackMetaData { get; set; } = "";
    [XmlElement(Namespace = "")] public string TrackURI { get; set; } = "";
    [XmlElement(Namespace = "")] public string RelTime { get; set; } = "";
    [XmlElement(Namespace = "")] public string AbsTime { get; set; } = "";
    [XmlElement(Namespace = "")] public string RelCount { get; set; } = "2147483647";
    [XmlElement(Namespace = "")] public string AbsCount { get; set; } = "2147483647";
}

public class GetTransportInfoResponse
{
    [XmlElement(Namespace = "")] public string CurrentTransportState { get; set; } = "";
    [XmlElement(Namespace = "")] public string CurrentTransportStatus { get; set; } = "";
    [XmlElement(Namespace = "")] public string CurrentSpeed { get; set; } = "";
}
