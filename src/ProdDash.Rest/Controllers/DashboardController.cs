using Microsoft.AspNetCore.Mvc;
using ProdDash.Api;
using TestKit;

namespace ProdDash.Rest.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ICache _cache;
    private readonly IQueries _queries;

    public DashboardController(ICache cache, IQueries queries)
    {
        _queries = queries;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<Schema.Dashboard>> Get(
        [FromQuery] string url = Constants.DefaultUrl,
        [FromQuery] double price = Constants.ExactPrice)
    {
        string msg;
        try
        {
            ArgumentException.ThrowIfNullOrEmpty(url);
            ArgumentException.ThrowIfNullOrEmpty(Convert.ToString(price));
            var pricex100 = Convert.ToInt32(double.Round(price, 2) * 100);
            var dta = await _cache.Refresh(url);
            var res = await _queries.GetDashboard(dta, pricex100);
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