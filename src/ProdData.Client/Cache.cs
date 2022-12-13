using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProdDash.Api;

namespace ProdData.Client;

public static class Inject
{
    public static IServiceCollection AddCache(this IServiceCollection services)
    {
        return services
            .AddEtlTasks()
            .AddTransient<ICache, Cache>();
    }
}

public class Cache: ICache
{
    private readonly GetJsonTask _getJsonTask;
    private readonly ExtractFunc _extract;
    private readonly TransformFunc _transform;
    private readonly LoadFunc _load;

    private IEnumerable<ProdDash.Api.Schema.FlatArticle>
        _dataDictionary = ImmutableList<ProdDash.Api.Schema.FlatArticle>.Empty.AsEnumerable();

    private DateTime _lastRefresh = DateTime.UtcNow;

    public Cache(
        GetJsonTask getJsonTask,
        ExtractFunc extract,
        TransformFunc transform,
        LoadFunc load)
    {
        _getJsonTask = getJsonTask;
        _extract = extract;
        _transform = transform;
        _load = load;
    }

    public Task<IEnumerable<ProdDash.Api.Schema.FlatArticle>> Refresh(string url,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(url))
            return (Task<IEnumerable<ProdDash.Api.Schema.FlatArticle>>)Task.CompletedTask;
        return Task<IEnumerable<ProdDash.Api.Schema.FlatArticle>>.Run(async () =>
        {
            try
            {
                var (_rawJson, _) = await _getJsonTask(url, cancellationToken).ConfigureAwait(false);
                lock (_dataDictionary)
                {
                    var prods = _extract(_rawJson);
                    _dataDictionary = _load(prods, _transform);
                }
                return _dataDictionary;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }, cancellationToken);
    }
}