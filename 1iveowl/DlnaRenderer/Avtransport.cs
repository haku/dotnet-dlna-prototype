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
    private readonly UpnpManager upnpManager;
    private readonly Device device;

    private bool ready = false;

    public Avtransport(UpnpManager upnpManager, IHostApplicationLifetime lifetime)
    {
        this.upnpManager = upnpManager;
        this.device = new Device(upnpManager.deviceCfg);

        lifetime.ApplicationStarted.Register(() => ready = true);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (!ready) await Task.Delay(1000);

            await device.StartAsync(cancellationToken);
            Console.WriteLine($"started: {upnpManager.deviceCfg.DeviceUUID}");
        });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await device?.ByeByeAsync();
        device?.Dispose();
    }

}
