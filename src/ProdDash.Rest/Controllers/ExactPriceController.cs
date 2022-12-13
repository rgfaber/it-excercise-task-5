using Microsoft.AspNetCore.Mvc;
using ProdDash.Api;
using TestKit;

namespace ProdDash.Rest.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ExactPriceController : ControllerBase
{
    private readonly ICache _cache;
    private readonly IQueries _queries;

    public ExactPriceController(
        ICache cache,
        IQueries queries)
    {
        _cache = cache;
        _queries = queries;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Schema.FlatArticle>>> Get(
        [FromQuery] string url = Constants.DefaultUrl,
        [FromQuery] double price = Constants.ExactPrice
    )
    {
        string msg;
        try
        {
            ArgumentException.ThrowIfNullOrEmpty(url);
            ArgumentException.ThrowIfNullOrEmpty(Convert.ToString(price));
            var pricex100 = Convert.ToInt32(double.Round(price, 2) * 100);
            var dta = await _cache.Refresh(url);
            var res = await _queries.GetExactPriceByPpLAsc(dta, pricex100);
            return Ok(res);
        }
        catch (Exception e)
        {
            msg = e.InnerAndOuter();
            Console.WriteLine(msg);
        }

        return BadRequest(msg);
    }
}