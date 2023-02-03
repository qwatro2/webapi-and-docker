using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;

namespace web.Controllers;

[ApiController]
[Route("latest_request")]
public class LatestRequestController : Controller
{
    private readonly ILogger<LatestRequestController> _logger;
    private readonly string _connectionString;

    public LatestRequestController(IOptions<PostgresOptions> postgresOptions,
        ILogger<LatestRequestController> logger)
    {
        _connectionString = postgresOptions.Value.ConnectionString;
        _logger = logger;
    }
    
    [HttpGet(Name = "latest_request")]
    public async Task<IActionResult> Get()
    {
        var conn = new NpgsqlConnection(_connectionString);
        await Solver.CreateTable(conn);
        var query = "SELECT * FROM public.requests ORDER BY id DESC LIMIT 1";

        try
        {
            var data = (await conn.QueryAsync(query)).ToArray()[0];
            IDictionary<string, object?> d0 = data;
            var d = new Dictionary<string, dynamic?>
            {
                { "RequestType", d0["request_type"] },
                { "RequestName", d0["request_name"] },
                { "Arguments", null },
                { "Result", d0["result"] },
                { "StatusCode", d0["status_code"] },
                { "Date", d0["date"] }
            };
            
            if (d0["arguments_id"] is not null)
            {
                query = $"SELECT * FROM public.arguments WHERE id = {d0["arguments_id"]}";
                var arguments = (await conn.QueryAsync(query)).ToArray()[0];
                IDictionary<string, object?> a = arguments;
                var args = new Dictionary<string, dynamic?>
                {
                    { "FirstArgument", a["first_argument"] },
                    { "SecondArgument", a["second_argument"] },
                    { "Operation", a["operation"] },
                };
                d["Arguments"] = args;
            }

            var res = JsonSerializer.Serialize(d);
            query = "INSERT INTO public.requests(request_type, request_name, result, status_code)" +
                    "VALUES ('GET', 'latest_request', @result::jsonb, 200)";

            await conn.ExecuteAsync(query, new { result = res });
            return Ok(res);
        }
        catch (Exception e)
        {
            query = "INSERT INTO public.requests(request_type, request_name, result, status_code)" +
                    "VALUES ('GET', 'latest_request', @result::jsonb, 400)";
        
            await conn.ExecuteAsync(query,
                new {result = JsonSerializer.Serialize(new {ErrorMessage = e.Message})});
            return BadRequest(e.Message);
        }
    }
}