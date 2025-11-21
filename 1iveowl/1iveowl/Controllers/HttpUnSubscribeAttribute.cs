using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Routing;

namespace _1iveowl.Controllers;

public class HttpUnSubscribeAttribute : HttpMethodAttribute
{
    private static readonly IEnumerable<string> _supportedMethods = ["UNSUBSCRIBE"];

    public HttpUnSubscribeAttribute()
        : base(_supportedMethods)
    {
    }

    public HttpUnSubscribeAttribute([StringSyntax("Route")] string template)
        : base(_supportedMethods, template)
    {
        ArgumentNullException.ThrowIfNull(template, nameof(template));
    }
}