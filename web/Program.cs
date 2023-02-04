namespace web;

internal static class Program
{
    private const string DockerUpCommand = "-f docker-compose.Development.yml up -d";
    private const string DockerDownCommand = "-f docker-compose.Development.yml down";
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.Configure<PostgresOptions>(
            builder.Configuration.GetSection(nameof(PostgresOptions)));
        
        var app = builder.Build();

        if (builder.Environment.IsDevelopment())
        {
            app.Lifetime.ApplicationStarted.Register(() => Solver.ExecuteCommand(DockerUpCommand));
            app.Lifetime.ApplicationStopped.Register(() => Solver.ExecuteCommand(DockerDownCommand));
        }

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}