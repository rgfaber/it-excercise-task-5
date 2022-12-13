using Microsoft.Extensions.DependencyInjection;
using ProdDash.Api;

namespace ProdData.Client;




public static class ProxyFuncs
{
    public static IServiceCollection AddProxy(this IServiceCollection services)
    {
        return services
            .AddTransient(_ => _getRemoteJson);
    }
    
    private static readonly GetJsonTask
        _getRemoteJson =
            async (url, ct) =>
            {
                Exception caught = null;
                var json = "[]";
                using var _httpClient = new HttpClient();
                try
                {
                    var rsp = await _httpClient
                        .GetAsync(url, ct)
                        .ConfigureAwait(false);
                    rsp.EnsureSuccessStatusCode();
                    json = await rsp.Content
                        .ReadAsStringAsync(ct)
                        .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    caught = e;
                    Console.WriteLine(e);
                }
                return (json, caught);
            };
}

