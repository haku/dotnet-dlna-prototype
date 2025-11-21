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
    private readonly SubscriptionManager subscriptionManager = new();

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

    // UPnP-arch-DeviceArchitecture-v2.0-20200417.pdf page 90

    // SUBSCRIBE http://192.168.1.66:5000/AVTransport/event HTTP/1.1
    // HOST: publisher host:publisher port
    // CALLBACK: <delivery URL>
    // NT: upnp:event
    // TIMEOUT: Second-requested subscription duration
    // STATEVAR: CSV of Statevariables

    // SUBSCRIBE /AVTransport/event HTTP/1.1
    // Timeout: Second-600
    // Nt: upnp:event
    // Callback: <http://192.168.1.5:43395/dev/6ec8fed7-918d-0ea1-3785-5bbaf277511b/svc/upnp-org/AVTransport/event/cb>
    // Host: 192.168.1.66:5000

    // HTTP/1.1 200 OK
    // SID: uuid:subscription-UUID
    // CONTENT-LENGTH: 0
    // TIMEOUT: Second-actual subscription duration
    // ACCEPTED-STATEVAR: CSV of state variables
    // (no body)

    // HTTP/1.1 200 OK
    // Date: Fri, 21 Nov 2025 19:34:24 GMT
    // Server: Linux/6.12.57 UPnP/1.0 jUPnP/3.0
    // Timeout: Second-1800
    // Sid: uuid:dc86de7b-c094-47e8-a018-20ff586feb03
    // Content-Length: 0

    // renew
    // SUBSCRIBE publisher path HTTP/1.1
    // HOST: publisher host:publisher port
    // SID: uuid:subscription UUID
    // TIMEOUT: Second-requested subscription duration

    // UNSUBSCRIBE publisher path HTTP/1.1
    // HOST: publisher host:publisher port
    // SID: uuid:subscription UUID

    [HttpSubscribe]
    [Route("AVTransport/event")]
    public async Task<IActionResult> AVTransportSubscribe()
    {
        string callbackUrl = Request.Headers["CALLBACK"].FirstOrDefault();
        string nt = Request.Headers["NT"].FirstOrDefault();
        string timeoutRaw = Request.Headers["TIMEOUT"].FirstOrDefault();
        int? timeoutSeconds = parseTimeout(timeoutRaw);
        string sid = Request.Headers["SID"].FirstOrDefault();
        string statevar = Request.Headers["STATEVAR"].FirstOrDefault();
        Console.WriteLine($"SUBSCRIBE: {callbackUrl} nt={nt} to={timeoutRaw} to={timeoutSeconds} sid={sid} statevar={statevar}");

        if (timeoutSeconds == null)
            return BadRequest("unknown value for TIMEOUT header.");

        if (sid != null)
        { // renew
            if (subscriptionManager.Renew(sid, timeoutSeconds.Value))
                return Ok();
            return BadRequest();
        }

        if (nt != "upnp:event")
            return BadRequest("unknown value for NT header.");

        var sub = subscriptionManager.Add(callbackUrl, timeoutSeconds.Value);

        Response.Headers.Add("Sid", sub.Sid);
        Response.Headers.Add("Timeout", $"Second-{timeoutSeconds}");
        return Ok();

        // TODO after finishing this request, send event with everything in it.
    }

    [HttpUnSubscribe]
    [Route("AVTransport/event")]
    public async Task<IActionResult> AVTransportUnSubscribe()
    {
        string sid = Request.Headers["SID"].FirstOrDefault();
        Console.WriteLine($"UNSUBSCRIBE: sid={sid}");

        if (sid == null)
            return BadRequest();

        if (subscriptionManager.Remove(sid))
            return Ok();
        return StatusCode(412); // DA spec if sub not found.
    }

    private int? parseTimeout(string raw)
    {
        if (raw.StartsWith("Second-"))
        {
            if (int.TryParse(raw.AsSpan("Second-".Length), out var ret))
            {
                return ret;
            }
        }
        return null;
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
