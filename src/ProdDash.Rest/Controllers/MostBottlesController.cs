using Microsoft.AspNetCore.Mvc;
using ProdDash.Api;
using TestKit;

namespace ProdDash.Rest.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class MostBottlesController : ControllerBase
{
    private readonly IQueries _queries;
    private readonly ICache _cache;

    public MostBottlesController(IQueries queries, ICache cache)
    {
        _queries = queries;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Schema.FlatArticle>>> Get(
        [FromQuery] string url = Constants.DefaultUrl
    )
    {
        string msg;
        try
        {
            ArgumentNullException.ThrowIfNullOrEmpty(url);
            var data = await _cache.Refresh(url);
            var res = _queries.GetMostBottles(data);
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