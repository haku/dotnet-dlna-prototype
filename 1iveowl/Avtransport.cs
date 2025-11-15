using System.Net;
using System.Security;
using System.Text;
using ISSDP.UPnP.PCL.Enum;
using ISSDP.UPnP.PCL.Interfaces.Model;
using Microsoft.AspNetCore.Mvc;
using SSDP.UPnP.PCL.Model;
using SSDP.UPnP.PCL.Service;

namespace _1iveowl;

public class Avtransport : IHostedService
{
    private readonly IRootDeviceConfiguration deviceConfig;
    private Device device;
    private bool ready = false;

    public Avtransport(IPAddress address, IHostApplicationLifetime lifetime)
    {
        deviceConfig = CreateRootDevice(address);
        lifetime.ApplicationStarted.Register(() => ready = true);
    }

    public void UseAvtransport(WebApplication app)
    {
        app.MapGet("/device", DescXml(deviceConfig));
        app.MapGet("/AVTransport/scpd.xml", ScpdXml(deviceConfig));
        app.MapPost("/AVTransport/control", AvtransportControl(deviceConfig));
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (!ready) await Task.Delay(1000);

            device = new Device(deviceConfig);
            await device.StartAsync(cancellationToken);
            Console.WriteLine($"started: {deviceConfig.DeviceUUID}");
        });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await device?.ByeByeAsync();
        device?.Dispose();
    }
    
    private static IRootDeviceConfiguration CreateRootDevice(IPAddress address)
    {
        return new RootDeviceConfiguration
        {
            DeviceUUID = Guid.NewGuid().ToString(),
            CacheControl = TimeSpan.FromSeconds(300),
            Location = new Uri($"http://{address.ToString()}:5000/device"),
            Server = new Server
            {
                UpnpMajorVersion = "2",
                UpnpMinorVersion = "0",
                ProductName = "My Example Renderer",
                IsUpnp2 = true
            },
            IpEndPoint = new IPEndPoint(address, 1901),
            // USN: uuid:a12e56ce-4df9-2ceb-12f3-cb586a8042f0::urn:schemas-upnp-org:device:MediaRenderer:1
            EntityType = EntityType.RootDevice,
            Domain = "schemas-upnp-org",
            TypeName = "MediaRenderer",
            Version = 1,
            CONFIGID = "100",
            Services = new List<IServiceConfiguration>
            {
                // USN: uuid:a12e56ce-4df9-2ceb-12f3-cb586a8042f0::urn:schemas-upnp-org:service:AVTransport:1
                new ServiceConfiguration
                {
                    EntityType = EntityType.DomainService,
                    Domain = "schemas-upnp-org",
                    TypeName = "AVTransport",
                    Version = 1,
                }
            }
        };
    }
    
    private static Delegate DescXml(IRootDeviceConfiguration config)
    {
        return (HttpContext ctx) =>
        {
            return $@"<?xml version=""1.0""?>
<root xmlns=""urn:schemas-upnp-org:device-1-0"">
  <specVersion><major>1</major><minor>0</minor></specVersion>
  <device>
    <deviceType>urn:schemas-upnp-org:device:MediaRenderer:1</deviceType>
    <friendlyName>Minimal DLNA Renderer</friendlyName>
    <manufacturer>Example</manufacturer>
    <modelDescription>Minimal</modelDescription>
    <modelName>MinimalRenderer</modelName>
    <modelNumber>v1</modelNumber>
    <UDN>{config.DeviceUUID}</UDN>
    <serviceList>
      <service> 
        <serviceType>urn:schemas-upnp-org:service:AVTransport:1</serviceType>
        <serviceId>urn:upnp-org:serviceId:AVTransport</serviceId>
        <controlURL>/AVTransport/control</controlURL>
        <SCPDURL>/AVTransport/scpd.xml</SCPDURL>
        <eventSubURL>/AVTransport/event</eventSubURL>
      </service>
    </serviceList>
  </device>
</root>";
        };
    }
    
    private static Delegate ScpdXml(IRootDeviceConfiguration config)
    {
        return (HttpContext ctx) =>
        {
            return $@"<?xml version=""1.0""?>
<scpd xmlns=""urn:schemas-upnp-org:service-1-0"">
  <specVersion><major>1</major><minor>0</minor></specVersion>
  <actionList>
    <action>
      <name>SetAVTransportURI</name>
      <argumentList>
        <argument><name>InstanceID</name><direction>in</direction></argument>
        <argument><name>CurrentURI</name><direction>in</direction></argument>
        <argument><name>CurrentURIMetaData</name><direction>in</direction></argument>
      </argumentList>
    </action>
    <action><name>Play</name></action>
    <action><name>Pause</name></action>
    <action><name>Stop</name></action>
    <action><name>Next</name></action>
    <action><name>Previous</name></action>
    <action><name>GetTransportInfo</name></action>
    <action><name>GetPositionInfo</name></action>
  </actionList> 
  <serviceStateTable>
  </serviceStateTable>
</scpd>";
        };
    }
    
    static string currentUri = "";
    static string transportState = "STOPPED";

    private static Delegate AvtransportControl(IRootDeviceConfiguration config)
    {
        return (HttpContext ctx, [FromBody] String body) =>
        {
            string soapAction = ctx.Request.Headers["SOAPACTION"].First() ?? "";
            Console.WriteLine($"SOAPAction header: {soapAction}");

            // naive parse: look for <u:ActionName or <ActionName
            string action = ParseSoapActionName(body);
            Console.WriteLine($"Action: {action}");

            string responseXml = null;
            int status = 200;

            switch (action)
            {
                case "SetAVTransportURI":
                {
                    currentUri = ExtractXmlTag(body, "CurrentURI") ?? "";
                    transportState = "STOPPED";
                    Console.WriteLine($"Set URI: {currentUri}");
                    responseXml = WrapEmptyActionResponse("SetAVTransportURIResponse");
                }
                    break;
                case "Play":
                    transportState = "PLAYING";
                    Console.WriteLine("Play command received");
                    responseXml = WrapEmptyActionResponse("PlayResponse");
                    break;
                case "Pause":
                    transportState = "PAUSED_PLAYBACK";
                    Console.WriteLine("Pause command received");
                    responseXml = WrapEmptyActionResponse("PauseResponse");
                    break;
                case "Stop":
                    transportState = "STOPPED";
                    Console.WriteLine("Stop command received");
                    responseXml = WrapEmptyActionResponse("StopResponse");
                    break;
                case "Next":
                    Console.WriteLine("Next command received");
                    responseXml = WrapEmptyActionResponse("NextResponse");
                    break;
                case "Previous":
                    Console.WriteLine("Previous command received");
                    responseXml = WrapEmptyActionResponse("PreviousResponse");
                    break;
                case "GetTransportInfo":
                    responseXml = transportInfoXml(transportState);
                    break;
                case "GetPositionInfo":
                    responseXml = positionInfoXml(currentUri);
                    break;
                default:
                    status = 500;
                    responseXml = soapError("401", "Action not implemented");
                    break;
            }

            byte[] outB = Encoding.UTF8.GetBytes(responseXml);
            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "text/xml; charset=\"utf-8\"";
            ctx.Response.Headers.ContentLength = outB.Length;
            ctx.Response.Body.Write(outB, 0, outB.Length);
            ctx.Response.Body.Close();
        };
    }
    
    static string ParseSoapActionName(string soapXml)
    {
        if (string.IsNullOrWhiteSpace(soapXml)) return "";
        // look for <u:ActionName or <ActionName
        int i = soapXml.IndexOf('<');
        while (i >= 0)
        {
            i = soapXml.IndexOf('<', i + 1);
            if (i < 0) break;
            int start = i + 1;
            if (start >= soapXml.Length) break;
            // skip slash
            if (soapXml[start] == '/') { continue; }
            int j = soapXml.IndexOfAny(new char[] { ' ', '>', ':' }, start);
            if (j < 0) continue;
            string tag = soapXml.Substring(start, j - start);
            // skip common envelope tags
            if (tag == "s:Envelope" || tag == "s:Body" || tag == "u:GetTransportInfo" || tag.EndsWith("Response")) continue;
            // return first plausible action name
            return tag.Contains(":") ? tag.Split(':')[1] : tag;
        }
        return "";
    }

    static string ExtractXmlTag(string xml, string tag)
    {
        string open = "<" + tag + ">";
        string close = "</" + tag + ">";
        int a = xml.IndexOf(open);
        int b = xml.IndexOf(close);
        if (a >= 0 && b > a)
            return WebUtility.HtmlDecode(xml.Substring(a + open.Length, b - (a + open.Length)));
        return null;
    }

    static string WrapEmptyActionResponse(string responseName) => $"<?xml version=\"1.0\"?>\n<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\n  <s:Body>\n    <u:{responseName} xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"/>\n  </s:Body>\n</s:Envelope>";

    static string transportInfoXml(string currentTransportState) => $@"<?xml version=""1.0""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
  <s:Body>          
    <u:GetTransportInfoResponse xmlns:u=""urn:schemas-upnp-org:service:AVTransport:1"">
      <CurrentTransportState>{SecurityElement.Escape(currentTransportState)}</CurrentTransportState>
      <CurrentTransportStatus>OK</CurrentTransportStatus>
      <CurrentSpeed>1</CurrentSpeed>
    </u:GetTransportInfoResponse>
  </s:Body>             
</s:Envelope>";

    static string positionInfoXml(string uri) => $@"<?xml version=""1.0""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
  <s:Body>
    <u:GetPositionInfoResponse xmlns:u=""urn:schemas-upnp-org:service:AVTransport:1"">
      <Track>1</Track>
      <TrackDuration>00:00:00</TrackDuration>
      <TrackMetaData></TrackMetaData>
      <TrackURI>{SecurityElement.Escape(uri)}</TrackURI>
      <RelTime>00:00:00</RelTime>
      <AbsTime>00:00:00</AbsTime>
    </u:GetPositionInfoResponse>
  </s:Body>
</s:Envelope>";

    static string soapError(string code, string desc) => $@"<?xml version=""1.0""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
  <s:Body>
    <s:Fault>
      <faultcode>s:Client</faultcode>
      <faultstring>UPnPError</faultstring>
      <detail>
        <UPnPError xmlns=""urn:schemas-upnp-org:control-1-0"">
          <errorCode>{code}</errorCode>
          <errorDescription>{SecurityElement.Escape(desc)}</errorDescription>
        </UPnPError>
      </detail>
    </s:Fault>
  </s:Body>
</s:Envelope>";

}