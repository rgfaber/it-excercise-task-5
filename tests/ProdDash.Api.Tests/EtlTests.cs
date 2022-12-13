using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using ProdData.Client;
using TestKit;
using Xunit.Abstractions;

namespace ProdDash.Api.Tests;

public class EtlTests : IoCTests
{
    private const string ProductDataResource = "ProdDash.Api.Tests.product_data.json";
    
    private string _inputJson;
   
    private TransformFunc _transform;
    private LoadFunc _load;
    private ExtractFunc _extract;
    
    private readonly ProdData.Client.Schema.Product _product = new(
        Id: 42,
        BrandName: "Tripple",
        Name: "Karmeliet",
        Articles: new[]
        {
            new ProdData.Client.Schema.Article(
                Id: 421,
                ShortDescription: "20 x 0,5L (Glas)",
                Price: 89.99,
                Unit: "Liter",
                PricePerUnitText: "(9,00 €/Liter)",
                Image: ""),
            new ProdData.Client.Schema.Article(
                Id: 422,
                ShortDescription: "6 x 0,3L (Can)",
                Price: 16.19,
                Unit: "Liter",
                PricePerUnitText: "(9,00 €/Liter)",
                Image: ""),
            new ProdData.Client.Schema.Article(
                Id: 423,
                ShortDescription: "6 x 0,25L (Can)",
                Price: 13.49,
                Unit: "Liter",
                PricePerUnitText: "(9,00 €/Liter)",
                Image: "")
        },
        DescriptionText: "Tripple Karmeliet aus Belgien");


    [Fact]
    public async Task ShouldReadProductDataJson()
    {
        // GIVEN
        Assert.NotNull(_inputJson);
        Assert.NotEmpty(_inputJson);
        Assert.True(_inputJson.IsJson());

        // WHEN
        _inputJson = TestHelper.GetEmbeddedResourceText(ProductDataResource);

        // THEN
        Assert.NotEmpty(_inputJson);
    }

    [Fact]
    public async Task ShouldDeserializeProductDataJson()
    {
        // GIVEN
        Assert.NotNull(_inputJson);
        Assert.NotEmpty(_inputJson);
        Assert.True(_inputJson.IsJson());

        // WHEN
        var res = _inputJson.FromJson<IImmutableList<ProdData.Client.Schema.Product>>();

        // THEN
        Assert.NotNull(res);
        Assert.NotEmpty(res);
    }

    [Fact]
    public async Task ShouldResolveExtractFunc()
    {
        // GIVEN
        Assert.NotNull(TestEnv);
        // WHEN
        _extract = TestEnv.ResolveRequired<ExtractFunc>();
        // THEN
        Assert.NotNull(_extract);
    }

    [Fact]
    public async Task ShouldResolveLoadFunc()
    {
        // GIVEN
        Assert.NotNull(TestEnv);
        // WHEN
        _load = TestEnv.ResolveRequired<LoadFunc>();
        // THEN
        Assert.NotNull(_load);
    }

    [Fact]
    public async Task ShouldResolveTransformFunc()
    {
        // GIVEN
        Assert.NotNull(TestEnv);
        // WHEN
        _transform = TestEnv.ResolveRequired<TransformFunc>();
        // THEN
        Assert.NotNull(_transform);
    }


    [Fact]
    public async Task ShouldExtractProductData()
    {
        // GIVEN
        Assert.NotNull(_inputJson);
        Assert.NotEmpty(_inputJson);
        Assert.True(_inputJson.IsJson());
        // AND
        _extract = TestEnv.ResolveRequired<ExtractFunc>();
        Assert.NotNull(_extract);
        // WHEN
        var res = _extract(_inputJson);
        // THEN
        Assert.True(res.Any());
        Assert.NotEmpty(res);
        // THEN
    }

    [Fact]
    public async Task ShouldTransformProductData()
    {
        // GIVEN
        
        Assert.NotNull(_inputJson);
        Assert.NotEmpty(_inputJson);
        Assert.True(_inputJson.IsJson());
        // AND
        _transform = TestEnv.ResolveRequired<TransformFunc>();
        Assert.NotNull(_transform);
        // WHEN
        var flatArticles = _transform(_product);
        // THEN
        Assert.NotNull(flatArticles);
        Assert.True(flatArticles.Any());
        Assert.Equal(3, flatArticles.Count);
    }

    [Fact]
    public async Task ShouldReturnMinDoubleValueForInvalidPpuText()
    {
        // GIVEM
        const string source = "(3,70 $/Gallon)";
        // WHEN
        var (ppu, parseRes) = source.ToPpu100();
        // THEN
        Assert.Equal(int.MinValue, ppu);
        Assert.NotEqual(RegExUtils.okay, parseRes);
    }

    [Fact]
    public async Task ShouldReturnSensibleValuesForNullPpuText()
    {
        // GIVEN
        const string source = null;
        // WHEN
        var (ppu, parseRes) = source.ToPpu100();
        // THEN
        Assert.NotEqual(RegExUtils.okay, parseRes);
    }
    
    [Fact]
    public async Task ShouldRegexReturnCorrectValueForValidPpuText()
    {
        // GIVEN
        const string source = "(3,70 €/Liter)";
        // WHEN
        var (ppu, parseRes) = source.ToPpu100();
        // THEN
        Assert.Equal(370, ppu);
        Assert.Equal(RegExUtils.okay, parseRes);
    }

    [Fact]
    public async Task ShouldLoadProductData()
    {
        // GIVEN
        // GIVEN
        Assert.NotNull(_inputJson);
        Assert.NotEmpty(_inputJson);
        Assert.True(_inputJson.IsJson());
        // AND
        _extract = TestEnv.ResolveRequired<ExtractFunc>();
        _transform = TestEnv.ResolveRequired<TransformFunc>();
        _load = TestEnv.ResolveRequired<LoadFunc>();
        // WHEN
        var products = _extract(_inputJson);
        // AND
        var res = _load(products, _transform);
        // THEN
        Assert.NotNull(res);
        Assert.True(res.Any());
    }

    public EtlTests(ITestOutputHelper output, IoCTestContainer testEnv) : base(output, testEnv)
    {
    }

    protected override void Initialize()
    {
        _inputJson = TestHelper.GetEmbeddedResourceText(ProductDataResource);
    }


    protected override void SetTestEnvironment()
    {
    }

    protected override void InjectDependencies(IServiceCollection services)
    {
        services
            .AddEtlTasks();
    }
}