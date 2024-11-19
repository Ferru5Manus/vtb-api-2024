using System.Reflection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.OpenSearch;

namespace MailService.Utils;

public static class Logging
{
    public static void configureLogging(){
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json",optional:false,reloadOnChange:true).Build();
        Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Debug()
                .WriteTo.Console()
                .Enrich.WithProperty("Environment",environment)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
    }
}