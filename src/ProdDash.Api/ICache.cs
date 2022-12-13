using System.Collections.Immutable;

namespace ProdDash.Api;


public interface ICache
{
    Task<IEnumerable<Schema.FlatArticle>> Refresh(string url, CancellationToken cancellationToken = default);
}

