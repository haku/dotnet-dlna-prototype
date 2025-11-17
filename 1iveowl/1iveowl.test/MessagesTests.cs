using _1iveowl.Models;

namespace _1iveowl.test;

public class MessagesTests
{
    [Fact]
    public void TestGetMediaInfoResponse()
    {
        Envelope envelope = new Envelope
        {
            Body = new Body
            {
                GetMediaInfoResponse = new GetMediaInfoResponse
                {
                    CurrentURI = "https://example.com/path/to/media",
                    CurrentURIMetaData = "<DIDL-Lite xmlns=\"urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:upnp=\"urn:schemas-upnp-org:metadata-1-0/upnp/\" xmlns:dlna=\"urn:schemas-dlna-org:metadata-1-0/\" xmlns:sec=\"http://www.sec.co.kr/\" xmlns:xbmc=\"urn:schemas-xbmc-org:metadata-1-0/\"><item id=\"https://example.com/path/to/media\"></item></DIDL-Lite>",
                    MediaDuration = "01:02:03",
                }
            }
        };
        CheckObjectAndResource(envelope, "Resources/GetMediaInfoResponse.xml");
    }

    [Fact]
    public void TestGetPositionInfoResponse()
    {
        Envelope envelope = new Envelope
        {
            Body = new Body
            {
                GetPositionInfoResponse = new GetPositionInfoResponse
                {
                    TrackDuration = "04:05:06",
                    TrackMetaData = "<DIDL-Lite></DIDL-Lite>",
                    TrackURI = "https://example.com/path/to/media",
                    RelTime = "01:02:03",
                    AbsTime = "03:02:01",
                }
            }
        };
        CheckObjectAndResource(envelope, "Resources/GetPositionInfoResponse.xml");
    }

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