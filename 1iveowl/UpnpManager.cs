using System.Net;
using ISSDP.UPnP.PCL.Enum;
using ISSDP.UPnP.PCL.Interfaces.Model;
using Microsoft.AspNetCore.Hosting.Server.Features;
using SSDP.UPnP.PCL.Model;

namespace _1iveowl;

public class UpnpManager
{
    internal readonly IRootDeviceConfiguration deviceCfg;
    internal readonly IPAddress address;

    public UpnpManager(Microsoft.AspNetCore.Hosting.Server.IServer server)
    {
        var addresses = server.Features.Get<IServerAddressesFeature>();
        var uri = new Uri(addresses.Addresses.First());
        address = IPAddress.Parse(uri.Host);
        deviceCfg = CreateRootDevice(address);
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
    
}