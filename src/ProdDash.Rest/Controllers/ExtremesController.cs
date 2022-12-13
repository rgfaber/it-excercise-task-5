using Microsoft.AspNetCore.Mvc;
using ProdDash.Api;
using TestKit;

namespace ProdDash.Rest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExtremesController : ControllerBase
{
    private readonly ICache _cache;
    private readonly IQueries _queries;

    public ExtremesController(
        IQueries queries,
        ICache cache
    )
    {
        _queries = queries;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<Schema.Extremes>> Get(
        [FromQuery] string url = Constants.DefaultUrl)
    {
        string msg;
        var res = Schema.Extremes.New();
        try
        {
            ArgumentException.ThrowIfNullOrEmpty(url);
            var dta = await _cache.Refresh(url);
            res = await _queries.GetExtremes(dta);
            return Ok(res);
        }
        catch (Exception e)
        {
            msg = e.InnerAndOuter();
            Console.WriteLine(msg);
        }
        return BadRequest(res);
    }
}