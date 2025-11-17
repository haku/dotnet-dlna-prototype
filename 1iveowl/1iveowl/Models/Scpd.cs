using System.Xml.Serialization;

namespace _1iveowl.Models;

[XmlRoot(ElementName = "scpd", Namespace = "urn:schemas-upnp-org:service-1-0")]
public class Scpd
{
    public SpecVersion specVersion = new();

    [XmlArray(ElementName = "actionList")]
    [XmlArrayItem(ElementName = "action")]
    public ScpdAction[] actions = [];

    [XmlArray(ElementName = "serviceStateTable")]
    [XmlArrayItem(ElementName = "stateVariable")]
    public StateVariable[] stateVariables = [];
}

public class SpecVersion
{
    public String major = "1";
    public String minor = "0";
}

public class ScpdAction
{
    public string name;

    [XmlArray(ElementName = "argumentList")]
    [XmlArrayItem(ElementName = "argument")]
    public Argument[] arguments = [];
}

public class Argument
{
    public string name;
    public string direction; // "in" or "out"
    public string relatedStateVariable;
}

public class StateVariable
{
    [XmlAttribute]
    public string sendEvents = "no";

    public string name;
    public string dataType;
    public string defaultValue;
    [XmlArray(ElementName = "allowedValueList")]
    [XmlArrayItem(ElementName = "allowedValue")]
    public string[] allowedValues;
}