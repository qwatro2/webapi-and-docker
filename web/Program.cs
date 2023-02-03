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
            // ExecuteCommand(dockerDownCommand);
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
        var processInfo = new ProcessStartInfo("docker-compose", command)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        int exitCode;
        using (var process = new Process())
        {
            process.StartInfo = processInfo;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit(1200000);
            if (!process.HasExited)
            {
                process.Kill();
            }

            exitCode = process.ExitCode;
            process.Close();
        }
    }
}
