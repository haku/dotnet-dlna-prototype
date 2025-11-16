using System.Xml.Serialization;
using _1iveowl.Models;

namespace _1iveowl.test;

public class ActionsTests
{
    [Fact]
    public void Test1()
    {
        string xml =
            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
            "<s:Envelope s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\" xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
            "<s:Body>" +
            "<u:Stop xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>0</InstanceID></u:Stop>" +
            "</s:Body>" +
            "</s:Envelope>";
        var xs = new XmlSerializer(typeof(Envelope));
        using var sr = new StringReader(xml);
        var obj = (Envelope) xs.Deserialize(sr);
        Assert.NotNull(obj.Body.Stop);
        Assert.Equal("0", obj.Body.Stop.InstanceID);
    }
}