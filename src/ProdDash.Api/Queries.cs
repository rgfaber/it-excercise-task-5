using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace ProdDash.Api;

public static class Inject
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        return services
            .AddTransient<IQueries, Queries>();
    }
}

internal class Queries : IQueries
{
    public Queries(ICache cache)
    {
    }


    public async Task<Schema.Extremes> GetExtremes(IEnumerable<Schema.FlatArticle> dta,
        CancellationToken cancellationToken = default)
    {
        if (!dta.Any())
            return Schema.Extremes.New();
        var min = dta.Min(x => x.PricePerUnitx100);
        var max = dta.Max(x => x.PricePerUnitx100);
        var enumerator = dta.GetEnumerator();
        var cheapest = ImmutableList<Schema.FlatArticle>.Empty;
        var mostExpensive = ImmutableList<Schema.FlatArticle>.Empty;
        var it = enumerator.MoveNext();
        if (!it)
            return Schema.Extremes.New();
        do
        {
            if (enumerator.Current.PricePerUnitx100 == min) cheapest = cheapest.Add(enumerator.Current);

            if (enumerator.Current.PricePerUnitx100 == max) mostExpensive = mostExpensive.Add(enumerator.Current);
        } while (enumerator.MoveNext());

        return Schema.Extremes.New(cheapest, mostExpensive);
    }


    public async Task<IEnumerable<Schema.FlatArticle>> GetExactPriceByPpLAsc(IEnumerable<Schema.FlatArticle> dta,
        int price, CancellationToken ct = default)
    {
        return dta
            .Where(a => a.Pricex100 == price)
            .OrderBy(a => a.PricePerUnitx100);
    }

    public async Task<IEnumerable<Schema.FlatArticle>> GetMostBottles(IEnumerable<Schema.FlatArticle> dta,
        CancellationToken ct = default)
    {
        var maxBottles = dta.Max(x => x.NumberOfBottles);
        return dta.Where(x => x.NumberOfBottles == maxBottles);
    }

    public async Task<Schema.Dashboard> GetDashboard(IEnumerable<Schema.FlatArticle> dta, int price,
        CancellationToken ct = default)
    {
        var extremes = await GetExtremes(dta, ct);
        var mostBottles = await GetMostBottles(dta, ct);
        var exact = await GetExactPriceByPpLAsc(dta, price, ct);
        return new Schema.Dashboard(extremes, exact, mostBottles);
    }
}