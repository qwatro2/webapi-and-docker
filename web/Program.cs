using Npgsql;
using web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<PostgresOptions>(
    builder.Configuration.GetSection(nameof(PostgresOptions)));

var app = builder.Build();

// docker run -d -p 5433:5432 postgres:latest

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();