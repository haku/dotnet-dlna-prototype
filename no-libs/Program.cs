using System.IO;
using System.Net.NetworkInformation ;
using System.Net.Sockets;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using System;

class MinimalDLNARenderer
{
    static string localIp;
    static int httpPort = 5000;
    static string deviceUuid = "uuid:d2d879d5-efa1-4866-9db1-314b9d433024";
    static string serviceType = "urn:schemas-upnp-org:service:AVTransport:1";

    static string descriptionXml => $@"<?xml version=""1.0""?>
<root xmlns=""urn:schemas-upnp-org:device-1-0"">
  <specVersion><major>1</major><minor>0</minor></specVersion>
  <device>
    <deviceType>urn:schemas-upnp-org:device:MediaRenderer:1</deviceType>
    <friendlyName>Minimal DLNA Renderer</friendlyName>
    <manufacturer>Example</manufacturer>
    <modelDescription>Minimal</modelDescription>
    <modelName>MinimalRenderer</modelName>
    <modelNumber>v1</modelNumber>
    <UDN>{deviceUuid}</UDN>
    <serviceList>
      <service>
        <serviceType>{serviceType}</serviceType>
        <serviceId>urn:upnp-org:serviceId:AVTransport</serviceId>
        <controlURL>/AVTransport/control</controlURL>
        <SCPDURL>/AVTransport/scpd.xml</SCPDURL>
        <eventSubURL>/AVTransport/event</eventSubURL>
      </service>
    </serviceList>
  </device>
</root>";

    static string scpdXml => $@"<?xml version=""1.0""?>
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
    <!-- Minimal state variables -->
  </serviceStateTable>
</scpd>";

    static string transportInfoXml(string currentTransportState) => $@"<?xml version=""1.0""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
  <s:Body>
    <u:GetTransportInfoResponse xmlns:u=""{serviceType}"">
      <CurrentTransportState>{SecurityElement.Escape(currentTransportState)}</CurrentTransportState>
      <CurrentTransportStatus>OK</CurrentTransportStatus>
      <CurrentSpeed>1</CurrentSpeed>
    </u:GetTransportInfoResponse>
  </s:Body>
</s:Envelope>";

    static string positionInfoXml(string uri) => $@"<?xml version=""1.0""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
  <s:Body>
    <u:GetPositionInfoResponse xmlns:u=""{serviceType}"">
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

    static string currentUri = "";
    static string transportState = "STOPPED";

    static async Task Main()
    {
        localIp = GetLocalIPAddress();
        Console.WriteLine($"Local IP: {localIp}");

        CancellationTokenSource cts = new CancellationTokenSource();

        var httpTask = RunHttpServer(cts.Token);
        var ssdpTask = RunSsdpResponder(cts.Token);

        Console.WriteLine("Renderer running. Press Ctrl+C to exit.");
        Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

        await Task.WhenAll(httpTask, ssdpTask);
    }

    static async Task RunHttpServer(CancellationToken ct)
    {
        var listener = new HttpListener();
        string prefix = $"http://{localIp}:{httpPort}/";
        listener.Prefixes.Add(prefix);
        listener.Start();
        Console.WriteLine($"HTTP server listening on {prefix}");

        while (!ct.IsCancellationRequested)
        {
            var ctx = await listener.GetContextAsync();
            _ = Task.Run(() => HandleHttp(ctx), ct);
        }

        listener.Stop();
    }

    static void HandleHttp(HttpListenerContext ctx)
    {
        try
        {
            string path = ctx.Request.Url.AbsolutePath;
            Console.WriteLine($"HTTP {ctx.Request.HttpMethod} {path}");

            if (path == "/description.xml")
            {
                byte[] b = Encoding.UTF8.GetBytes(descriptionXml);
                ctx.Response.ContentType = "text/xml; charset=us-ascii";
                ctx.Response.ContentLength64 = b.Length;
                ctx.Response.OutputStream.Write(b, 0, b.Length);
                ctx.Response.OutputStream.Close();
                return;
            }
            if (path == "/AVTransport/scpd.xml")
            {
                byte[] b = Encoding.UTF8.GetBytes(scpdXml);
                ctx.Response.ContentType = "text/xml";
                ctx.Response.ContentLength64 = b.Length;
                ctx.Response.OutputStream.Write(b, 0, b.Length);
                ctx.Response.OutputStream.Close();
                return;
            }

            if (path == "/AVTransport/control")
            {
                // expecting SOAP action in the body
                string body;
                using (var sr = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding))
                    body = sr.ReadToEnd();

                string soapAction = ctx.Request.Headers["SOAPACTION"] ?? "";
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
                ctx.Response.ContentLength64 = outB.Length;
                ctx.Response.OutputStream.Write(outB, 0, outB.Length);
                ctx.Response.OutputStream.Close();
                return;
            }

            // unknown path
            ctx.Response.StatusCode = 404;
            ctx.Response.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("HTTP handle error: " + ex);
        }
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

    static string WrapEmptyActionResponse(string responseName) => $"<?xml version=\"1.0\"?>\n<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\n  <s:Body>\n    <u:{responseName} xmlns:u=\"{serviceType}\"/>\n  </s:Body>\n</s:Envelope>";

    static async Task RunSsdpResponder(CancellationToken ct)
    {
        using (var udp = new UdpClient())
        {
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 1900);
            udp.Client.Bind(localEp);

            udp.JoinMulticastGroup(IPAddress.Parse("239.255.255.250"));
            Console.WriteLine("SSDP responder listening on port 1900");

            // send initial NOTIFY once
            _ = Task.Run(() => SendNotifyLoop(ct));

            while (!ct.IsCancellationRequested)
            {
                var result = await udp.ReceiveAsync();
                string msg = Encoding.UTF8.GetString(result.Buffer);
                if (msg.StartsWith("M-SEARCH", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Received M-SEARCH from " + result.RemoteEndPoint);
                    try
                        {
                            if (msg.IndexOf("ssdp:all", StringComparison.OrdinalIgnoreCase) >= 0
                                || msg.IndexOf("MediaRenderer", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                string resp = $"HTTP/1.1 200 OK\r\n"
                                    + "ST: {serviceType}\r\n"
                                    + "USN: {deviceUuid}::{serviceType}\r\n"
                                    + "LOCATION: http://{localIp}:{httpPort}/description.xml\r\n"
                                    + "CACHE-CONTROL: max-age=1800\r\n\r\n";
                                byte[] b = Encoding.UTF8.GetBytes(resp);
                                await udp.SendAsync(b, b.Length, result.RemoteEndPoint);
                                Console.WriteLine("Sent M-SEARCH response to " + result.RemoteEndPoint);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Failed to sed M-SEARCH response to {result.RemoteEndPoint}: {e}");
                        }
                }
            }
        }
    }

    static async Task SendNotifyLoop(CancellationToken ct)
    {
        using (var udp = new UdpClient())
        {
            var multicast = new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900);
            while (!ct.IsCancellationRequested)
            {
                string notify1 = $"NOTIFY * HTTP/1.1\r\n"
                    + "LOCATION: http://{localIp}:{httpPort}/description.xml\r\n"
                    + "CACHE-CONTROL: max-age=300\r\n"
                    + "NT: upnp:rootdevice\r\n"
                    + "HOST: 239.255.255.250:1900\r\n"
                    + "NTS: ssdp:alive\r\n"
                    + "USN: {deviceUuid}::upnp:rootdevice\r\n"
                    + "SERVER: MinimalDLNARenderer/1.0 UPnP/1.0\r\n\r\n";
                byte[] b1 = Encoding.UTF8.GetBytes(notify1);
                try { await udp.SendAsync(b1, b1.Length, multicast); } catch { }
                await Task.Delay(TimeSpan.FromSeconds(1), ct).ContinueWith(_ => { });

                string notify2 = $"NOTIFY * HTTP/1.1\r\n"
                    + "HOST: 239.255.255.250:1900\r\n"
                    + "NT: {serviceType}\r\n"
                    + "NTS: ssdp:alive\r\n"
                    + "USN: {deviceUuid}::{serviceType}\r\n"
                    + "LOCATION: http://{localIp}:{httpPort}/description.xml\r\n"
                    + "CACHE-CONTROL: max-age=1800\r\n"
                    + "SERVER: MinimalDLNARenderer/1.0 UPnP/1.0\r\n\r\n";
                byte[] b2 = Encoding.UTF8.GetBytes(notify2);
                try { await udp.SendAsync(b2, b2.Length, multicast); } catch { }
                await Task.Delay(TimeSpan.FromSeconds(15), ct).ContinueWith(_ => { });
            }
        }
    }

    static string GetLocalIPAddress()
    {
        foreach (NetworkInterface network in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (network.NetworkInterfaceType != NetworkInterfaceType.Ethernet
                || network.OperationalStatus != OperationalStatus.Up
                || new[] { "docker", "tailscale" }.Any(c => network.Description.ToLower().Contains(c)))
            {
              continue;
            }

            foreach (IPAddressInformation address in network.GetIPProperties().UnicastAddresses)
            {
              if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                continue;

              if (IPAddress.IsLoopback(address.Address))
                continue;

              return address.Address.ToString();
            }
        }
        return "127.0.0.1";
    }
}
