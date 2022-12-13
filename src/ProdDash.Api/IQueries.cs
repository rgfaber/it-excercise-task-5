namespace ProdDash.Api;

public interface IQueries
{
    Task<Schema.Extremes> GetExtremes(IEnumerable<Schema.FlatArticle> dta, CancellationToken ct = default);
    Task<IEnumerable<Schema.FlatArticle>> GetExactPriceByPpLAsc(IEnumerable<Schema.FlatArticle> dta, int price, CancellationToken ct = default);
    Task<IEnumerable<Schema.FlatArticle>> GetMostBottles(IEnumerable<Schema.FlatArticle> dta, CancellationToken ct = default);
    Task<Schema.Dashboard> GetDashboard(IEnumerable<Schema.FlatArticle> dta, int price, CancellationToken ct = default);
}