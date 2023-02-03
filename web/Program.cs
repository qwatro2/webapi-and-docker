using System.Diagnostics;

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

            app.Lifetime.ApplicationStopped.Register(() => ExecuteCommand(DockerDownCommand));
            ExecuteCommand(DockerUpCommand);
            RegisterApp(app);
        }
        else
        {
            RegisterApp(app);
        }
    }

    private static void RegisterApp(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }

    private static void ExecuteCommand(string command)
    {
        using (var process = new Process
               {
                   StartInfo =
                   {
                       FileName = "docker-compose",
                       WorkingDirectory = @"C:\Users\Public\prog\hse\ср доскер\web",
                       Arguments = command
                   }
               })
        {
            process.Start();
            process.Close();
        }
    }
}