using System.Net;
using Microsoft.Extensions.DependencyInjection;
using ProdDash.Api;
using TestKit;
using Xunit.Abstractions;

namespace ProdData.Client.Tests;

public class ProxyTests : IoCTests
{
    private const string ValidUrl = "https://flapotest.blob.core.windows.net/test/ProductData.json";
    private const string InvalidUrl = "https://flapotest.blob.core.windows.net/giggledi/ProductData.json";
    private const string EmptyUrl = "";


    private GetJsonTask _getJson;


    public ProxyTests(ITestOutputHelper output, IoCTestContainer testEnv) : base(output, testEnv)
    {
    }

    [Fact]
    public void ShouldResolveGetProdDataTask()
    {
        // GIVEN
        Assert.NotNull(TestEnv);
        // WHEN
        _getJson = TestEnv.ResolveRequired<GetJsonTask>();
        // THEN
        Assert.NotNull(_getJson);
    }

    [Fact]
    public async Task ShouldReturnInvalidOperationExceptionForEmptyUrl()
    {
        // GIVEN
        Assert.NotNull(TestEnv);
        _getJson = TestEnv.ResolveRequired<GetJsonTask>();
        Assert.NotNull(_getJson);
        var cts = new CancellationTokenSource(1_000);

        // WHEN
        var (json, ex) = await _getJson(EmptyUrl, cts.Token);

        //THEN
        Assert.Equal("[]", json);
        Assert.NotNull(ex);
        Assert.IsType<InvalidOperationException>(ex);
    }

    [Fact]
    public async Task ShouldReturnHttpRequestExceptionForInvalidUrl()
    {
        // GIVEN
        Assert.NotNull(TestEnv);
        _getJson = TestEnv.ResolveRequired<GetJsonTask>();
        Assert.NotNull(_getJson);
        var cts = new CancellationTokenSource(1_000);

        // WHEN
        var (json, ex) = await _getJson(InvalidUrl, cts.Token);

        //THEN
        Assert.Equal("[]", json);
        Assert.NotNull(ex);
        Assert.IsType<HttpRequestException>(ex);
        Assert.Equal(HttpStatusCode.NotFound, ((HttpRequestException)ex).StatusCode);
    }

    [Fact]
    public async Task ShouldReturnValidJsonForValidUrl()
    {
        // GIVEN
        Assert.NotNull(TestEnv);
        _getJson = TestEnv.ResolveRequired<GetJsonTask>();
        Assert.NotNull(_getJson);
        var cts = new CancellationTokenSource(1_000);

        // WHEN
        var (json, ex) = await _getJson(ValidUrl, cts.Token);

        //THEN
        Assert.NotEqual("[]", json);
        Assert.Null(ex);
        Assert.True(json.IsJson());
    }

    protected override void Initialize()
    {
    }

    protected override void SetTestEnvironment()
    {
    }

    protected override void InjectDependencies(IServiceCollection services)
    {
        services
            .AddProxy();
    }
}