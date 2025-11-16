using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace _1iveowl.Models;

public class XmlToString
{
    public static string SerializeObject<T>(T toSerialize)
    {
        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        ns.Add("s", "http://schemas.xmlsoap.org/soap/envelope/");
        ns.Add("u", "urn:schemas-upnp-org:service:AVTransport:1");

        XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
        using MemoryStream stream = new MemoryStream();
        using var xmlWriter = XmlTextWriter.Create(stream, new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            Indent = true,
        });
        xmlSerializer.Serialize(xmlWriter, toSerialize, ns);
        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
