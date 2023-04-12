using ArchivedSalesFileCleaner.Services;
using ArchivedSalesFileCleaner.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text;

internal class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");

        var configuration = builder.Build();


        Log.Logger = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration)
                                .CreateLogger();


        try
        {
            Log.Information($"=====>>>> Starting up <<<<<=====");

            var serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog(dispose: true);
                })
                .AddSingleton<IConfiguration>(configuration)
                .Configure<DeleteSettings>(configuration.GetSection("DeleteSettings"))
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

            // Generate summary
            var totalFilesSelected = filesToDelete.Count;
            var totalFilesDeleted = deletionResponses.Count(r => r.fileOperationType == FileOperationType.Delete);
            var totalFilesArchived = deletionResponses.Count(r => r.fileOperationType == FileOperationType.Archive);
            var totalErrors = deletionResponses.Count(r => r.fileOperationStatus == FileOperationStatus.Error);

            var summary = new StringBuilder();
           
            summary.AppendLine($"Total files selected: {totalFilesSelected}");
            summary.AppendLine($"Total files deleted: {totalFilesDeleted}");
            summary.AppendLine($"Total files archived: {totalFilesArchived}");
            summary.AppendLine($"Total errors: {totalErrors}");
            summary.AppendLine(Environment.NewLine);
          
         
            Log.Information("\n======================================================\n");
            Log.Information("Summary: {@Summary}", new
            {
                TotalFilesSelected = totalFilesSelected,
                TotalFilesDeleted = totalFilesDeleted,
                TotalFilesArchived = totalFilesArchived,
                TotalErrors = totalErrors
            });

            Log.Information("\n======================================================");
         
            Log.Information("Shutting down\n\n");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly\n\n");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
