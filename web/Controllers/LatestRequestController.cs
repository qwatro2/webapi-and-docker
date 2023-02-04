using System.Text.Json;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;
using web.Models;

namespace web.Controllers;

[ApiController]
[Route("latest_request")]
public class LatestRequestController : Controller
{
    private readonly ILogger<LatestRequestController> _logger;
    private readonly NpgsqlConnection _connection;

    private const string SelectQuery = "SELECT * FROM public.requests ORDER BY id DESC LIMIT 1";
    private const string SelectArgumentsQuery = "SELECT * FROM public.arguments WHERE id = @arguments_id";
    private const string InsertOkQuery = @"INSERT INTO public.requests(request_type, request_name, result, status_code) 
                                           VALUES ('GET', 'latest_request', @result::jsonb, 200)";
    private const string InsertBadQuery = @"INSERT INTO public.requests(request_type, request_name, result, status_code)
                                            VALUES ('GET', 'latest_request', @result::jsonb, 400)";
    
    public LatestRequestController(IOptions<PostgresOptions> postgresOptions,
        ILogger<LatestRequestController> logger)
    {
        _connection = new NpgsqlConnection(postgresOptions.Value.ConnectionString);
        _logger = logger;
    }
    
    [HttpGet(Name = "latest_request")]
    public async Task<IActionResult> Get()
    {
        await Solver.CreateTable(_connection);
        
        try
        {
            var data = (await _connection.QueryAsync(SelectQuery)).ToArray()[0];
            RequestDataModel requestDataModel = RequestDataModel.CreateRequestDataModel(data);

            if (requestDataModel.ArgumentsId is not null)
            {
                var arguments = (await _connection.QueryAsync(SelectArgumentsQuery,
                    new {arguments_id = requestDataModel.ArgumentsId})).ToArray()[0];
                ArgumentsModel argumentsModel = ArgumentsModel.CreateArgumentsModel(arguments);
                requestDataModel.Arguments = argumentsModel;
            }

            var res = JsonSerializer.Serialize(requestDataModel);
            await _connection.ExecuteAsync(InsertOkQuery, new { result = res });
            return Ok(res);
        }
        catch (Exception e)
        {
            await _connection.ExecuteAsync(InsertBadQuery,
                new { result = JsonSerializer.Serialize(new {ErrorMessage = e.Message}) });

            return BadRequest(e.Message);
        }
    }
}