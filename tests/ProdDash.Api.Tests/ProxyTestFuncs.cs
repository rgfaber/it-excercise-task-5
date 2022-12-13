using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace ProdDash.Api.Tests;

public static class ProxyTestFuncs
{
    private const string RawJsonResourceName = "ProdDash.Api.Tests.product_data.json";


    private static readonly GetJsonTask
        _getEmbeddedJson =
            async (_, _) =>
            {
                Exception caught = null;
                var result = "[]";
                try
                {
                    var assy = typeof(ProxyTestFuncs).Assembly;
                    var sIn = assy.GetManifestResourceStream(RawJsonResourceName);
                    result = StreamToString(sIn);
                }
                catch (Exception e)
                {
                    caught = e;
                }

                return (result, caught);
            };

    public static IServiceCollection AddTestProxy(this IServiceCollection services)
    {
        return services
            .AddTransient(_ => _getEmbeddedJson);
    }

    private static string StreamToString(this Stream stream)
    {
        if (stream == null) return "";
        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}