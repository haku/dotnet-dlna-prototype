using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using _1iveowl;
using ISSDP.UPnP.PCL.Enum;
using ISSDP.UPnP.PCL.Interfaces.Model;
using ISSDP.UPnP.PCL.Interfaces.Service;
using SSDP.UPnP.PCL.ExtensionMethod;
using SSDP.UPnP.PCL.Helper;
using SSDP.UPnP.PCL.Model;
using SSDP.UPnP.PCL.Service;

class Program
{
    private static IControlPoint _controlPoint;
    private static IPAddress _controlPointLocalIp1;

    private static async Task Main(string[] args)
    {
        if (args.Length > 0)
        {
            var ipStr = args[0];
            if (IPAddress.TryParse(ipStr, out var ip))
            {
                _controlPointLocalIp1 = ip;
            }
        }

        if (_controlPointLocalIp1 is null)
        {
            _controlPointLocalIp1 = GetLocalIPAddress();
        }

        Console.WriteLine($"IP Address: {_controlPointLocalIp1.ToString()}");

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<UpnpManager>();
        builder.Services.AddHostedService<Avtransport>();
        builder.Services.AddControllers().AddXmlSerializerFormatters();

        var app = builder.Build();
        app.MapControllers();
        app.Urls.Add($"http://{_controlPointLocalIp1.ToString()}:5000");

        // var cts = new CancellationTokenSource();
        // var ct = cts.Token;
        // await StartControlPointListeningAsync(ct);

        await app.RunAsync();
    }

    static IPAddress GetLocalIPAddress()
    {
        foreach (var network in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (network.NetworkInterfaceType != NetworkInterfaceType.Ethernet
                || network.OperationalStatus != OperationalStatus.Up
                || new[] { "docker", "tailscale" }.Any(c => network.Description.ToLower().Contains(c)))
            {
                continue;
            }

            foreach (var address in network.GetIPProperties().UnicastAddresses)
            {
                if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                    continue;

                if (IPAddress.IsLoopback(address.Address))
                    continue;

                return address.Address;
            }
        }
        return IPAddress.Parse("127.0.0.1");
    }

    private static async Task StartControlPointListeningAsync(CancellationToken ct)
    {
        _controlPoint = new ControlPoint(_controlPointLocalIp1);
        _controlPoint.Start(ct);
        ListenToNotify();
        ListenToMSearchResponse(ct);
        await StartMSearchRequestMulticastAsync();
    }

    private static void ListenToNotify()
    {
        var observerNotify = _controlPoint.NotifyObservable();
        var counter = 0;
        var disposableNotify = observerNotify
            .Subscribe(
                n =>
                {
                    counter++;
                    Console.WriteLine($"---### Control Point Received a NOTIFY - #{counter} ###---");
                    Console.WriteLine($"{n?.NotifyTransportType.ToString()}");
                    Console.WriteLine($"From: {n?.HOST}");
                    Console.WriteLine($"Location: {n?.Location?.AbsoluteUri}");
                    Console.WriteLine($"Cache-Control: max-age = {n.CacheControl}");
                    Console.WriteLine($"Server: " +
                                             $"{n?.Server?.OperatingSystem}/{n?.Server?.OperatingSystemVersion} " +
                                             $"UPNP/" +
                                             $"{n?.Server?.UpnpMajorVersion}.{n?.Server?.UpnpMinorVersion}" +
                                             $" " +
                                             $"{n?.Server?.ProductName}/{n?.Server?.ProductVersion}" +
                                             $" - ({n?.Server?.FullString})");
                    Console.WriteLine($"NT: {n?.NT}");
                    Console.WriteLine($"NTS: {n?.NTS}");
                    Console.WriteLine($"USN: {n?.USN?.ToUri()}");

                    if (n.BOOTID > 0)
                    {
                        Console.WriteLine($"BOOTID: {n.BOOTID}");
                    }

                    Console.WriteLine($"CONFIGID: {n.CONFIGID}");

                    Console.WriteLine($"NEXTBOOTID: {n.NEXTBOOTID}");
                    Console.WriteLine($"SEARCHPORT: {n.SEARCHPORT}");
                    Console.WriteLine($"SECURELOCATION: {n.SECURELOCATION}");

                    if (n.Headers.Any())
                    {
                        Console.WriteLine($"Additional Headers: {n.Headers.Count}");
                        foreach (var header in n.Headers)
                        {
                            Console.WriteLine($"{header.Key}: {header.Value}; ");
                        }
                    }

                    Console.WriteLine($"Is UPnP 2.0 compliant: {n.IsUuidUpnp2Compliant}");

                    if (n.HasParsingError)
                    {
                        Console.WriteLine($"Parsing errors: {n.HasParsingError}");
                    }

                    Console.WriteLine();
                });
    }

    private static void ListenToMSearchResponse(CancellationToken ct)
    {
        var mSearchResObs = _controlPoint.MSearchResponseObservable();
        var counter = 0;
        var disposableMSearchresponse = mSearchResObs
            .Subscribe(
                res =>
                {
                    counter++;
                    Console.WriteLine($"---### Control Point Received a M-SEARCH RESPONSE #{counter} ###---");
                    Console.WriteLine($"{res?.TransportType.ToString()}");
                    Console.WriteLine($"Status code: {res.StatusCode} {res.ResponseReason}");
                    Console.WriteLine($"Location: {res?.Location?.AbsoluteUri}");
                    Console.WriteLine($"Date: {res.Date.ToString(CultureInfo.CurrentCulture)}");
                    Console.WriteLine($"Cache-Control: max-age = {res.CacheControl}");
                    // Console.WriteLine($"Server: " +
                    //                          $"{res?.Server?.OperatingSystem}/{res?.Server?.OperatingSystemVersion} " +
                    //                          $"UPNP/" +
                    //                          $"{res?.Server?.UpnpMajorVersion}.{res?.Server?.UpnpMinorVersion}" +
                    //                          $" " +
                    //                          $"{res?.Server?.ProductName}/{res?.Server?.ProductVersion}" +
                    //                          $" - ({res?.Server?.FullString})");
                    Console.WriteLine($"ST: {res?.ST?.STString}  EntityType={res?.ST?.EntityType}");
                    Console.WriteLine($"USN: {res.USN?.ToUri()}");
                    // Console.WriteLine($"BOOTID.UPNP.ORG: {res?.BOOTID}");
                    // Console.WriteLine($"CONFIGID.UPNP.ORG: {res?.CONFIGID}");
                    // Console.WriteLine($"SEARCHPORT.UPNP.ORG: {res?.SEARCHPORT}");
                    // Console.WriteLine($"SECURELOCATION: {res?.SECURELOCATION}");

                    if (res?.Headers?.Any() ?? false)
                    {
                        Console.WriteLine($"Additional Headers: {res.Headers?.Count}");
                        foreach (var header in res.Headers)
                        {
                            Console.WriteLine($"{header.Key}: {header.Value}; ");
                        }
                    }

                    if (res.HasParsingError)
                    {
                        Console.WriteLine($"Parsing errors: {res.HasParsingError}");
                    }

                    Console.WriteLine();
                });
    }

    private static async Task StartMSearchRequestMulticastAsync()
    {
        var mSearchMessage = new MSearch
        {
            TransportType = TransportType.Multicast,
            CPFN = "TestXamarin",
            Name = Constants.UdpSSDPMultiCastAddress,
            Port = Constants.UdpSSDPMulticastPort,
            MX = TimeSpan.FromSeconds(5),
            TCPPORT = Constants.TcpResponseListenerPort.ToString(),
            ST = new ST
            {
                StSearchType = STType.All
            },
            UserAgent = new UserAgent
            {
                ProductName = "SSDP.UPNP.PCL",
                ProductVersion = "0.9",
                UpnpMajorVersion = "2",
                UpnpMinorVersion = "0",
            }
        };

        await _controlPoint.SendMSearchAsync(mSearchMessage, _controlPointLocalIp1);
    }

}

internal class MSearch : IMSearchRequest
{
    public bool InvalidRequest { get; } = false;
    public bool HasParsingError { get; internal set; }
    public string Name { get; internal set; }
    public int Port { get; internal set; }
    public IDictionary<string, string> Headers { get; internal set; }
    public TransportType TransportType { get; internal set; }
    public string MAN { get; internal set; }
    public string HOST { get; internal set; }
    public TimeSpan MX { get; internal set; }
    public IST ST { get; internal set; }
    public IUserAgent UserAgent { get; internal set; }
    public string CPFN { get; internal set; }
    public string CPUUID { get; internal set; }
    public int SEARCHPORT { get; internal set; }
    public string TCPPORT { get; internal set; }
    public IPEndPoint LocalIpEndPoint { get; internal set; }
    public IPEndPoint RemoteIpEndPoint { get; internal set; }
}

internal class UserAgent : IUserAgent
{
    public string FullString { get; set; }
    public string OperatingSystem { get; set; }
    public string OperatingSystemVersion { get; set; }
    public string ProductName { get; set; }
    public string ProductVersion { get; set; }
    public string UpnpMajorVersion { get; set; }
    public string UpnpMinorVersion { get; set; }
    public bool IsUpnp2 { get; set; }
}
