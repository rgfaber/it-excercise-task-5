namespace ProdDash.Api;

public delegate Task<(string,Exception)> 
    GetJsonTask(
        string url,
        CancellationToken cancellationToken = default);