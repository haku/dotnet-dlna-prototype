Run
---

$ nix-shell
$ dotnet run

Notes
-----

llm suggested next steps:
* Use RSSDP (or ExSsdp) for the SSDP advertisement/discovery parts. It’s lightweight and well-suited for that layer.
* Combine with a minimal HTTP + SOAP handler (e.g. ASP.NET Core or HttpListener) for the control service endpoints (AVTransport, RenderingControl).
* Use or adapt genielabs/intel-upnp-dlna (or the “UPnP” NuGet package) as a support layer for parsing service definitions, generating SOAP envelopes, and maybe eventing if available.
* Write your own glue layer for state, event notifications, LastChange, etc.

but...
* https://github.com/yortw/RSSDP is 5 years old.
* https://github.com/genielabs/intel-upnp-dlna is 2 years old.
