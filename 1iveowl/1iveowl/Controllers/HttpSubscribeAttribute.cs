using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Routing;

namespace _1iveowl.Controllers;

public class HttpSubscribeAttribute : HttpMethodAttribute
{
    private static readonly IEnumerable<string> _supportedMethods = ["SUBSCRIBE"];

    public HttpSubscribeAttribute()
        : base(_supportedMethods)
    {
    }

    public HttpSubscribeAttribute([StringSyntax("Route")] string template)
        : base(_supportedMethods, template)
    {
        ArgumentNullException.ThrowIfNull(template, nameof(template));
    }
}