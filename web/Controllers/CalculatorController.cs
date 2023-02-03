using System.Text.Json;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;

namespace web.Controllers;

[ApiController]
[Route("calculate")]
public class CalculatorController : Controller
{
    private readonly ILogger<CalculatorController> _logger;
    private readonly string _connectionString;

    public CalculatorController(IOptions<PostgresOptions> postgresOptions,
        ILogger<CalculatorController> logger)
    {
        _connectionString = postgresOptions.Value.ConnectionString;
        _logger = logger;
    }
    
    [HttpPost(Name = "calculate")]
    public async Task<IActionResult> Post(double first, double second, char operation)
    {
        var op = Solver.FromStringToOperation(operation);
        double? res =  op switch
        {
            Operation.Undefined => null,
            Operation.Division when second is 0 => null,
            Operation.Plus => first + second,
            Operation.Minus => first - second,
            Operation.Multiplication => first * second,
            Operation.Division => first / second,
            _ => null
        };
        
        var statusCode = res is null ? 400 : 200;
        
        var conn = new NpgsqlConnection(_connectionString);
        await Solver.CreateTable(conn);
        const string query = "WITH rows AS (" +
                             "INSERT INTO public.arguments (first_argument, second_argument, operation) " +
                             "VALUES (@first, @second, @operation) " +
                             "RETURNING id" +
                             ")" +
                             "INSERT INTO public.requests(request_type, request_name, arguments_id, result, status_code) " +
                             "VALUES ('POST', 'calculate', (SELECT id FROM rows), @result::jsonb, @statusCode)";
        await conn.ExecuteAsync(
            query, new
            {
                first,
                second,
                operation,
                result = JsonSerializer.Serialize(new { Result = res}),
                statusCode
            });

        return res is null ? BadRequest() : Ok(res);
    }
}