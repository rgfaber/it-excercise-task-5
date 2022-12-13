using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace ProdData.Client;

public delegate
    IImmutableList<Schema.Product> ExtractFunc(
        string rawJson);

public delegate
    IImmutableList<ProdDash.Api.Schema.FlatArticle> TransformFunc(
        Schema.Product product);

public delegate 
    IEnumerable<ProdDash.Api.Schema.FlatArticle> LoadFunc( 
        IImmutableList<Schema.Product> products, 
        TransformFunc transform);


public static class EtlFuncs
{

    public static IServiceCollection AddEtlTasks(this IServiceCollection services)
    {
        return services
            .AddTransient(_ => _extract)
            .AddTransient(_ => _transform)
            .AddTransient(_ => _load);
    }

    private static readonly ExtractFunc
        _extract =
            raw =>
                raw.FromJson<ImmutableList<Schema.Product>>();


    private static readonly TransformFunc
        _transform =
            product =>
            {
                return product.Articles.Aggregate(
                    ImmutableList<ProdDash.Api.Schema.FlatArticle>.Empty,
                    (current, article) =>
                    {
                        /// We round doubles to 2 decimals and x 100,
                        /// invest some performance now,
                        /// in order to save on it while querying.
                        var (ppu, parseRes) = article.PricePerUnitText.ToPpu100();
                        var price100 = Convert.ToInt32(double.Round(article.Price, 2) * 100);
                        var (nbrOfBottles, parseBottles) = article.ShortDescription.ToNbrOfBottles();
                        return current.Add(new ProdDash.Api.Schema.FlatArticle(
                            product.Id,
                            product.Name,
                            article.Id,
                            article.ShortDescription,
                            price100,
                            article.Unit,
                            article.PricePerUnitText,
                            ppu,
                            $"{parseRes}, {parseBottles}" ,
                            nbrOfBottles));
                    });
            };



    private static readonly LoadFunc
        _load =
            (products, transform) =>
            {
                return products.Aggregate(
                    ImmutableArray<ProdDash.Api.Schema.FlatArticle>.Empty,
                    (current, product) =>
                        current.AddRange(transform(product)))
                    .AsQueryable();
            };
}