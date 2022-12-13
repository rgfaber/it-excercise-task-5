using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using ProdData.Client;
using TestKit;
using Xunit.Abstractions;

namespace ProdDash.Api.Tests;





public class QueryTests : IoCTests
{

    private const string ProdDataResourceName = "ProdDash.Api.Tests.product_data.json";
    
    private GetJsonTask _getData;
    private string _rawJson;
    private LoadFunc _load;
    private ExtractFunc _extract;
    private IImmutableList<ProdData.Client.Schema.Product> _prodData;
    private TransformFunc _transform;
    private IEnumerable<Schema.FlatArticle> _flatArticles;
    private IQueries _queries;


    [Fact]
    public async Task ShouldResolveGetProdDataTask()
    {
        // GIVEN
        Assert.NotNull(_getData);
        // WHEN
        _getData = TestEnv.ResolveRequired<GetJsonTask>();
        // THEN 
        Assert.NotNull(_getData);
    }
    
    [Fact]
    public async Task ShouldExecuteGetExtremes()
    {
        // GIVEN
        Assert.NotNull(TestEnv);
        var cts = new CancellationTokenSource(1_000);
        // AND
        _queries = TestEnv.ResolveRequired<IQueries>();
        Assert.NotNull(_queries);
        // AND
        _load = TestEnv.ResolveRequired<LoadFunc>();
        _extract = TestEnv.ResolveRequired<ExtractFunc>();
        _transform = TestEnv.ResolveRequired<TransformFunc>();
        var (_rawJson, caught) = await _getData("", cts.Token);
        if (caught != null) throw caught;
        
        _prodData = _extract(_rawJson); // AND
        _flatArticles = _load(_prodData, _transform);
        // WHEN
        var extremes = await _queries.GetExtremes(_flatArticles, cts.Token);
        // THEN
        Assert.NotNull(extremes);
    }
    
    
    [Fact]
    public async Task ShouldExecuteGetMostBottles()
    {
        // GIVEN
        Assert.NotNull(TestEnv);
        var cts = new CancellationTokenSource(1_000);
        // AND
        _queries = TestEnv.ResolveRequired<IQueries>();
        Assert.NotNull(_queries);
        // AND
        _load = TestEnv.ResolveRequired<LoadFunc>();
        _extract = TestEnv.ResolveRequired<ExtractFunc>();
        _transform = TestEnv.ResolveRequired<TransformFunc>();
        var (_rawJson, caught) = await _getData("", cts.Token);
        if (caught != null) throw caught;
        
        _prodData = _extract(_rawJson); // AND
        _flatArticles = _load(_prodData, _transform);
        // WHEN
        var mostBottles = await _queries.GetMostBottles(_flatArticles, cts.Token);
        // THEN
        Assert.NotNull(mostBottles);
    }
    
    
    [Fact]
    public async Task ShouldExecuteExactPriceByPpLAsc()
    {
        // GIVEN
        Assert.NotNull(TestEnv);
        var cts = new CancellationTokenSource(1_000);
        // AND
        _queries = TestEnv.ResolveRequired<IQueries>();
        Assert.NotNull(_queries);
        // AND
        _load = TestEnv.ResolveRequired<LoadFunc>();
        _extract = TestEnv.ResolveRequired<ExtractFunc>();
        _transform = TestEnv.ResolveRequired<TransformFunc>();
        var (_rawJson, caught) = await _getData("", cts.Token);
        if (caught != null) throw caught;
        
        _prodData = _extract(_rawJson); // AND
        _flatArticles = _load(_prodData, _transform);
        // WHEN
        var exactPriceByPpLAsc = await _queries.GetExactPriceByPpLAsc(_flatArticles, 1799, cts.Token);
        // THEN
        Assert.NotNull(exactPriceByPpLAsc);
    }
    
    [Fact]
    public async Task ShouldExecuteGetDashboard()
    {
        // GIVEN
        Assert.NotNull(TestEnv);
        var cts = new CancellationTokenSource(1_000);
        // AND
        _queries = TestEnv.ResolveRequired<IQueries>();
        Assert.NotNull(_queries);
        // AND
        _load = TestEnv.ResolveRequired<LoadFunc>();
        _extract = TestEnv.ResolveRequired<ExtractFunc>();
        _transform = TestEnv.ResolveRequired<TransformFunc>();
        var (_rawJson, caught) = await _getData("", cts.Token);
        if (caught != null) throw caught;
        
        _prodData = _extract(_rawJson); // AND
        _flatArticles = _load(_prodData, _transform);
        // WHEN
        var dashboard = await _queries.GetDashboard(_flatArticles, 1799, cts.Token);
        // THEN
        Assert.NotNull(dashboard);
    }
    

    
    
    
    
    

    [Fact]
    public void ShouldResolveQueries()
    {
        // GIVEN
        Assert.NotNull(TestEnv);
        // WHEN
        _queries = TestEnv.ResolveRequired<IQueries>();
        // THEN
        Assert.NotNull(_queries);
    }

    public QueryTests(ITestOutputHelper output, IoCTestContainer testEnv) : base(output, testEnv)
    {}

    protected override void Initialize()
    {
        _getData = TestEnv.ResolveRequired<GetJsonTask>();        
    }

    protected override void SetTestEnvironment()
    {}

    protected override void InjectDependencies(IServiceCollection services)
    {
        services
            .AddTestProxy()
            .AddCache()
            .AddEtlTasks()
            .AddQueries();
            

    }
}