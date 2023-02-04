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
    private readonly NpgsqlConnection _connection;

    private const string Query = @"WITH rows AS (
        INSERT INTO public.arguments (first_argument, second_argument, operation) 
        VALUES (@first, @second, @operation) 
        RETURNING id
        )
        INSERT INTO public.requests(request_type, request_name, arguments_id, result, status_code) 
        VALUES ('POST', 'calculate', (SELECT id FROM rows), @result::jsonb, @statusCode)";

    public CalculatorController(IOptions<PostgresOptions> postgresOptions,
        ILogger<CalculatorController> logger)
    {
        _connection = new NpgsqlConnection(postgresOptions.Value.ConnectionString);
        _logger = logger;
    }
    
    [HttpPost(Name = "calculate")]
    public async Task<IActionResult> Post(double first, double second, char operation)
    {
        double? res =  operation switch
        {
            '/' when second is 0 => null,
            '+' => first + second,
            '-' => first - second,
            '*' => first * second,
            '/' => first / second,
            _ => null
        };
        
        var statusCode = res is null ? 400 : 200;
        
        await Solver.CreateTable(_connection);
        
        await _connection.ExecuteAsync(
            Query, new
            {
                first,
                second,
                operation,
                result = JsonSerializer.Serialize(new { Result = res }),
                statusCode
            });

        return res is null ? BadRequest() : Ok(res);
    }
}