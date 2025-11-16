using System.Xml.Serialization;
using _1iveowl.Models;

namespace _1iveowl.test;

public class MessagesTests
{
    [Fact]
    public void TestGetTransportInfoResponse()
    {
        Envelope envelope = new Envelope
        {
            Body = new Body
            {
                GetTransportInfoResponse = new GetTransportInfoResponse
                {
                    CurrentTransportState = "STOPPED",
                    CurrentTransportStatus = "OK",
                    CurrentSpeed = "1"
                }
            }
        };
        CheckObjectAndResource(envelope, "Resources/GetTransportInfoResponse.xml");
    }

    public static void CheckObjectAndResource(object obj, string resourcePath)
    {
        string expected = File.ReadAllText(resourcePath);
        string actual = XmlToString.SerializeObject(obj);
        Assert.Equal(expected, actual);
    }

}