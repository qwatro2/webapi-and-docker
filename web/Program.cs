using System.Diagnostics;
using web;

class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<PostgresOptions>(
            builder.Configuration.GetSection(nameof(PostgresOptions)));

        var app = builder.Build();

        if (builder.Environment.IsDevelopment())
        {
            const string dockerUpCommand = "-f docker-compose.Development.yml up -d";
            const string dockerDownCommand = "-f docker-compose.Development.yml down";
            
            ExecuteCommand(dockerUpCommand);
            RegisterApp(app);
            ExecuteCommand(dockerDownCommand);
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
