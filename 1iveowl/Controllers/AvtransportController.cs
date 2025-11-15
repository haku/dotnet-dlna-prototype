using System.Net;
using System.Security;
using Microsoft.AspNetCore.Mvc;

namespace _1iveowl.Controllers;

[ApiController]
[Produces("text/xml")]
public class AvtransportController : ControllerBase
{
    private readonly string CONTENT_TYPE = "text/xml; charset=\"utf-8\"";
    private readonly UpnpManager upnpManager;

    public AvtransportController(UpnpManager upnpManager)
    {
        this.upnpManager = upnpManager;
    }

    [HttpGet]
    [Route("device")]
    public IActionResult Device()
    {
        return Content($@"<?xml version=""1.0""?>
<root xmlns=""urn:schemas-upnp-org:device-1-0"">
  <specVersion><major>1</major><minor>0</minor></specVersion>
  <device>
    <deviceType>urn:schemas-upnp-org:device:MediaRenderer:1</deviceType>
    <friendlyName>Minimal DLNA Renderer</friendlyName>
    <manufacturer>Example</manufacturer>
    <modelDescription>Minimal</modelDescription>
    <modelName>MinimalRenderer</modelName>
    <modelNumber>v1</modelNumber>
    <UDN>{upnpManager.deviceCfg.DeviceUUID}</UDN>
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
</root>", CONTENT_TYPE);
    }

    [HttpGet]
    [Route("AVTransport/scpd.xml")]
    public IActionResult AVTransportScpd()
    {
        return Content($@"<?xml version=""1.0""?>
<scpd xmlns=""urn:schemas-upnp-org:service-1-0"">
  <specVersion><major>1</major><minor>0</minor></specVersion>
  <actionList>
    <action>
      <name>SetAVTransportURI</name>
      <argumentList>
        <argument><name>InstanceID</name><direction>in</direction><relatedStateVariable>A_ARG_TYPE_InstanceID</relatedStateVariable></argument>
        <argument><name>CurrentURI</name><direction>in</direction><relatedStateVariable>AVTransportURI</relatedStateVariable></argument>
        <argument><name>CurrentURIMetaData</name><direction>in</direction><relatedStateVariable>AVTransportURIMetaData</relatedStateVariable></argument>
      </argumentList>
    </action>
    <action>
      <name>Play</name>
      <argumentList>
        <argument><name>InstanceID</name><direction>in</direction><relatedStateVariable>A_ARG_TYPE_InstanceID</relatedStateVariable></argument>
        <argument><name>Speed</name><direction>in</direction><relatedStateVariable>TransportPlaySpeed</relatedStateVariable></argument>
      </argumentList>
    </action>
    <action>
      <name>Pause</name>    
      <argumentList>
        <argument><name>InstanceID</name><direction>in</direction><relatedStateVariable>A_ARG_TYPE_InstanceID</relatedStateVariable></argument>
      </argumentList>
    </action>
    <action>
      <name>Stop</name>
      <argumentList>
        <argument><name>InstanceID</name><direction>in</direction><relatedStateVariable>A_ARG_TYPE_InstanceID</relatedStateVariable></argument>
      </argumentList>
    </action>
    <action>
      <name>Next</name>
      <argumentList>
        <argument><name>InstanceID</name><direction>in</direction><relatedStateVariable>A_ARG_TYPE_InstanceID</relatedStateVariable></argument>
      </argumentList>
    </action>
    <action>
      <name>Previous</name>
      <argumentList>
        <argument><name>InstanceID</name><direction>in</direction><relatedStateVariable>A_ARG_TYPE_InstanceID</relatedStateVariable></argument>
      </argumentList>
    </action>

    <action>
        <name>GetTransportInfo</name>
        <argumentList>
        <argument>
        <name>InstanceID</name>
        <direction>in</direction>
        <relatedStateVariable>A_ARG_TYPE_InstanceID</relatedStateVariable>
        </argument>
        <argument>
        <name>CurrentTransportState</name>
        <direction>out</direction>
        <relatedStateVariable>TransportState</relatedStateVariable>
        </argument>
        <argument>
        <name>CurrentTransportStatus</name>
        <direction>out</direction>
        <relatedStateVariable>TransportStatus</relatedStateVariable>
        </argument>
        <argument>
        <name>CurrentSpeed</name>
        <direction>out</direction>
        <relatedStateVariable>TransportPlaySpeed</relatedStateVariable>
        </argument>
        </argumentList>
    </action>

    <action>
        <name>GetPositionInfo</name>
        <argumentList>
            <argument>
            <name>InstanceID</name>
            <direction>in</direction>
            <relatedStateVariable>A_ARG_TYPE_InstanceID</relatedStateVariable>
            </argument>
            <argument>
            <name>Track</name>
            <direction>out</direction>
            <relatedStateVariable>CurrentTrack</relatedStateVariable>
            </argument>
            <argument>
            <name>TrackDuration</name>
            <direction>out</direction>
            <relatedStateVariable>CurrentTrackDuration</relatedStateVariable>
            </argument>
            <argument>
            <name>TrackMetaData</name>
            <direction>out</direction>
            <relatedStateVariable>CurrentTrackMetaData</relatedStateVariable>
            </argument>
            <argument>
            <name>TrackURI</name>
            <direction>out</direction>
            <relatedStateVariable>CurrentTrackURI</relatedStateVariable>
            </argument>
            <argument>
            <name>RelTime</name>
            <direction>out</direction>
            <relatedStateVariable>RelativeTimePosition</relatedStateVariable>
            </argument>
            <argument>
            <name>AbsTime</name>
            <direction>out</direction>
            <relatedStateVariable>AbsoluteTimePosition</relatedStateVariable>
            </argument>
            <argument>
            <name>RelCount</name>
            <direction>out</direction>
            <relatedStateVariable>RelativeCounterPosition</relatedStateVariable>
            </argument>
            <argument>
            <name>AbsCount</name>
            <direction>out</direction>
            <relatedStateVariable>AbsoluteCounterPosition</relatedStateVariable>
            </argument>
        </argumentList>
    </action>
  </actionList> 
  <serviceStateTable>
    <stateVariable sendEvents=""no"">
        <name>A_ARG_TYPE_InstanceID</name>
        <dataType>ui4</dataType>
    </stateVariable>
    <stateVariable sendEvents=""no"">
        <name>AVTransportURI</name>
        <dataType>string</dataType>
    </stateVariable>
    <stateVariable sendEvents=""no"">
        <name>AVTransportURIMetaData</name>
        <dataType>string</dataType>
        <defaultValue>NOT_IMPLEMENTED</defaultValue>
    </stateVariable>

    <stateVariable sendEvents=""no"">
        <name>TransportState</name>
        <dataType>string</dataType>
        <allowedValueList>
            <allowedValue>STOPPED</allowedValue>
            <allowedValue>PLAYING</allowedValue>
            <allowedValue>TRANSITIONING</allowedValue>
            <allowedValue>PAUSED_PLAYBACK</allowedValue>
            <allowedValue>PAUSED_RECORDING</allowedValue>
            <allowedValue>RECORDING</allowedValue>
            <allowedValue>NO_MEDIA_PRESENT</allowedValue>
            <allowedValue>CUSTOM</allowedValue>
        </allowedValueList>
    </stateVariable>

    <stateVariable sendEvents=""no"">
        <name>TransportStatus</name>
        <dataType>string</dataType>
        <allowedValueList>
            <allowedValue>OK</allowedValue>
            <allowedValue>ERROR_OCCURRED</allowedValue>
            <allowedValue>CUSTOM</allowedValue>
        </allowedValueList>
    </stateVariable>

    <stateVariable sendEvents=""no"">
        <name>TransportPlaySpeed</name>
        <dataType>string</dataType>
        <defaultValue>1</defaultValue>
    </stateVariable>

<stateVariable sendEvents=""no"">
<name>CurrentTrack</name>
<dataType>ui4</dataType>
<defaultValue>0</defaultValue>
</stateVariable>
<stateVariable sendEvents=""no"">
<name>CurrentTrackDuration</name>
<dataType>string</dataType>
</stateVariable>
<stateVariable sendEvents=""no"">
<name>CurrentTrackMetaData</name>
<dataType>string</dataType>
<defaultValue>NOT_IMPLEMENTED</defaultValue>
</stateVariable>
<stateVariable sendEvents=""no"">
<name>CurrentTrackURI</name>
<dataType>string</dataType>
</stateVariable>

<stateVariable sendEvents=""no"">
<name>RelativeTimePosition</name>
<dataType>string</dataType>
</stateVariable>
<stateVariable sendEvents=""no"">
<name>AbsoluteTimePosition</name>
<dataType>string</dataType>
</stateVariable>
<stateVariable sendEvents=""no"">
<name>RelativeCounterPosition</name>
<dataType>i4</dataType>
<defaultValue>2147483647</defaultValue>
</stateVariable>
<stateVariable sendEvents=""no"">
<name>AbsoluteCounterPosition</name>
<dataType>i4</dataType>
<defaultValue>2147483647</defaultValue>
</stateVariable>

  </serviceStateTable>
</scpd>", CONTENT_TYPE);
    }

    string currentUri = "";
    string transportState = "STOPPED";
    
    [HttpPost]
    [Route("AVTransport/control")]
    public async Task<IActionResult> AVTransportControl()
    {
        if (!Request.Body.CanSeek) Request.EnableBuffering();
        Request.Body.Position = 0;
        using var reader = new StreamReader(Request.Body); // TODO fix encoding.
        string body = await reader.ReadToEndAsync();
        
        string soapAction = Request.Headers["SOAPACTION"].First() ?? "";
            Console.WriteLine($"SOAPAction header: {soapAction}");

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

            var resp = Content(responseXml, CONTENT_TYPE);
            resp.StatusCode = status;
            return resp;
    }

    static string ParseSoapActionName(string soapXml)
    {
        if (string.IsNullOrWhiteSpace(soapXml)) return "";
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