using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace _1iveowl.Models;

public class XmlToString
{
    public static string SerializeObject<T>(T toSerialize)
    {
        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        if (toSerialize is Scpd)
        {
            ns.Add("", "urn:schemas-upnp-org:service-1-0");
        } else {
            ns.Add("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.Add("u", "urn:schemas-upnp-org:service:AVTransport:1");
        }

        XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
        using MemoryStream stream = new MemoryStream();
        using var xmlWriter = new XmlTextWriter(stream, new UTF8Encoding(false));
        xmlWriter.Formatting = Formatting.Indented;

        xmlSerializer.Serialize(xmlWriter, toSerialize, ns);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

}
