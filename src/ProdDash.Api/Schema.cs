using System.Collections.Immutable;

namespace ProdDash.Api;

public static class Schema
{
    public record FlatArticle(
        int ProductId,
        string Name,
        int ArticleId,
        string ShortDescription,
        int Pricex100,
        string Unit,
        string PricePerUnitText,
        int PricePerUnitx100,
        string ParseResult,
        int NumberOfBottles
    );


    public record Extremes(
        IImmutableList<FlatArticle> CheapestByPpu,
        IImmutableList<FlatArticle> MostExpensiveByPpu
    )
    {
        public static Extremes New()
        {
            return new Extremes(
                ImmutableList<FlatArticle>.Empty,
                ImmutableList<FlatArticle>.Empty);
        }

        public static Extremes New(
            IImmutableList<FlatArticle> minima,
            IImmutableList<FlatArticle> maxima)
        {
            return new Extremes(
                minima,
                maxima);
        }
    }


    public record Dashboard(
        Extremes Extremes,
        IEnumerable<FlatArticle> ExactByPrice,
        IEnumerable<FlatArticle> MostBottles
    );
}