using ArchivedSalesFileCleaner.Services;
using ArchivedSalesFileCleaner.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

internal class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
     .AddJsonFile("appsettings.json")
     .Build();

        Log.Logger = new LoggerConfiguration()
     .ReadFrom.Configuration(configuration.GetSection("Serilog"))
     .CreateLogger();

        try
        {
            Log.Information("Starting up");
            var serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog(dispose: true);
                })
                .AddSingleton<IConfiguration>(configuration).Configure<DeleteSettings>(configuration.GetSection("DeleteSettings"))
                .AddTransient<FileSelectionService>()
                .AddTransient<FileOperationService>()
                .BuildServiceProvider();

            var fileSelectionService = serviceProvider.GetService<FileSelectionService>();
            var fileOperationService = serviceProvider.GetService<FileOperationService>();

            if (fileSelectionService == null || fileOperationService == null)
            {
                throw new ArgumentNullException("Services are not being initialized properly");
            }

            var filesToDelete = fileSelectionService.GetFiles();
            var deletionResponses = fileOperationService.ProcessFiles(filesToDelete);

            // Rest of the code...

            Log.Information("Shutting down");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}