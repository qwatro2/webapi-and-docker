using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;

namespace web.Controllers;

[ApiController]
[Route("")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly NpgsqlConnection _connection;

    public HomeController(IOptions<PostgresOptions> postgresOptions,
        ILogger<HomeController> logger)
    {
        _connection = new NpgsqlConnection(postgresOptions.Value.ConnectionString);
        _logger = logger;
    }

    [HttpGet(Name = "home")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Get()
    {
        await Solver.CreateTable(_connection);
        return Redirect("/swagger");
    }
}