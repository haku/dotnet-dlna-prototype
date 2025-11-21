using _1iveowl.Models;
using _1iveowl.test;

namespace _1iveowl.test;

public class SubscriptionsTests
{
    [Fact]
    public void TestLastChange()
    {
        PropertySet propertySet = new PropertySet
        {
            Property = new Property
            {
                LastChange = XmlToString.SerializeObject(new Event
                {
                    InstanceID = new()
                    {
                        val = "0",
                        TransportState = new() { val = "PLAYING" },
                    }
                }, false, false),
            }
        };
        MessagesTests.CheckObjectAndResource(propertySet, "Resources/notification-wrapped.xml");
    }

    [Fact]
    public void TestLastChangeFull()
    {
        PropertySet propertySet = new PropertySet
        {
            Property = new Property
            {
                LastChange = XmlToString.SerializeObject(new Event
                {
                    InstanceID = new()
                    {
                        val = "0",
                        AVTransportURI = new() { val = "https://example.com/path/to/media" },
                        AVTransportURIMetaData = new() { val = "metadata" },
                        CurrentMediaDuration = new() { val = "00:00:00" },
                        CurrentPlayMode = new() { val = "NORMAL" },
                        CurrentTrack = new() { val = "1" },
                        CurrentTrackDuration = new() { val = "00:03:11" },
                        CurrentTrackMetaData = new() { val = "metadata" },
                        CurrentTrackURI = new() { val = "https://example.com/path/to/media" },
                        CurrentTransportActions = new() { val = "Stop,Pause,Seek,Next" },
                        TransportPlaySpeed = new() { val = "1" },
                        TransportState = new() { val = "PLAYING" },
                        TransportStatus = new() { val = "OK" },
                    }
                }, false, false),
            }
        };
        MessagesTests.CheckObjectAndResource(propertySet, "Resources/notification-wrapped-full.xml");
    }
}