using System.Text.Json.Serialization;

namespace ProdData.Client;

public static class Schema
{
    public record Product(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("brandName")]
        string BrandName,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("articles")]
        IReadOnlyList<Article> Articles,
        [property: JsonPropertyName("descriptionText")]
        string DescriptionText
    );

    public record Article(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("shortDescription")]
        string ShortDescription,
        [property: JsonPropertyName("price")] double Price,
        [property: JsonPropertyName("unit")] string Unit,
        [property: JsonPropertyName("pricePerUnitText")]
        string PricePerUnitText,
        [property: JsonPropertyName("image")] string Image
    );
}