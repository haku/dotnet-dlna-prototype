using System.Net;
using System.Security;
using _1iveowl.Models;
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
        return Content(XmlToString.SerializeObject(AvtransportScpd.SCPD), CONTENT_TYPE);
    }

    string currentUri = "";
    string transportState = "STOPPED";
    
    [HttpPost]
    [Route("AVTransport/control")]
    public async Task<IActionResult> AVTransportControl([FromBody] Envelope envelope)
    {
        string soapAction = Request.Headers["SOAPACTION"].First() ?? "";
        Console.WriteLine($"SOAPAction header: {soapAction}");

        var body = envelope.Body;
        string? action = null;
        if (body.SetAVTransportURI != null)
        {
            action = "SetAVTransportURI";
        }
        else if (body.Play != null)
        {
            action = "Play";
        }
        else if (body.Pause != null)
        {
            action = "Pause";
        }
        else if (body.Stop != null)
        {
            action = "Stop";
        }
        else if (body.Next != null)
        {
            action = "Next";
        }
        else if (body.Previous != null)
        {
            action = "Previous";
        }
        else if (body.GetTransportInfo != null)
        {
            action = "GetTransportInfo";
        }
        else if (body.GetPositionInfo != null)
        {
            action = "GetPositionInfo";
        }
        else if (body.GetMediaInfo != null)
        {
            action = "GetMediaInfo";
        }
        
        Console.WriteLine($"Action: {action}");

        string responseXml = null;
        int status = 200;

        switch (action)
        {
            case "SetAVTransportURI":
                currentUri = body.SetAVTransportURI.CurrentURI ?? "";
                transportState = "STOPPED";
                Console.WriteLine($"Set URI: {currentUri}");
                responseXml = WrapEmptyActionResponse("SetAVTransportURIResponse");
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
                responseXml = XmlToString.SerializeObject(new Envelope
                {
                    Body = new Body
                    {
                        GetTransportInfoResponse = new GetTransportInfoResponse
                        {
                            CurrentTransportState = transportState,
                            CurrentTransportStatus = "OK",
                            CurrentSpeed = "1"
                        }
                    }
                });
                break;
            case "GetPositionInfo":
                responseXml = XmlToString.SerializeObject(new Envelope
                {
                    Body = new Body
                    {
                        GetPositionInfoResponse = new GetPositionInfoResponse
                        {
                            TrackURI = currentUri,
                        }
                    }
                });
                break;
            case "GetMediaInfo":
                responseXml = XmlToString.SerializeObject(new Envelope
                {
                    Body = new Body
                    {
                        GetMediaInfoResponse = new GetMediaInfoResponse
                        {
                            CurrentURI = currentUri,
                        }
                    }
                });
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

    static string WrapEmptyActionResponse(string responseName) => $"<?xml version=\"1.0\"?>\n<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\n  <s:Body>\n    <u:{responseName} xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"/>\n  </s:Body>\n</s:Envelope>";

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
