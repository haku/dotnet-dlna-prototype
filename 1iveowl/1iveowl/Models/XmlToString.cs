using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace _1iveowl.Models;

public class XmlToString
{
    public static string SerializeObject<T>(T toSerialize, bool declaration = true, bool indent = true)
    {
        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        if (toSerialize is Scpd)
        {
            ns.Add("", "urn:schemas-upnp-org:service-1-0");
        } else if (toSerialize is PropertySet) {
            ns.Add("e", "urn:schemas-upnp-org:event-1-0");
        } else if (toSerialize is Event) {
            ns.Add("", "urn:schemas-upnp-org:metadata-1-0/AVT/");
        } else {
            ns.Add("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.Add("u", "urn:schemas-upnp-org:service:AVTransport:1");
        }

        XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
        using MemoryStream stream = new MemoryStream();

        // using var xmlWriter = new XmlTextWriter(stream, new UTF8Encoding(false));
        // xmlWriter.Formatting = Formatting.Indented;

        using var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            OmitXmlDeclaration = !declaration,
            Indent = indent,
        });

        xmlSerializer.Serialize(xmlWriter, toSerialize, ns);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

}
