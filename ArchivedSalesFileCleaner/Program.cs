using ArchivedSalesFileCleaner.Services;
using ArchivedSalesFileCleaner.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace ArchivedSalesFileCleaner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            var serviceProvider = new ServiceCollection()
                .Configure<DeleteSettings>(configuration.GetSection("DeleteSettings"))
                .AddTransient<FileSelectionService>()
                .BuildServiceProvider();

            var fileSelectionService = serviceProvider.GetService<FileSelectionService>();

            if (fileSelectionService == null)
            {
                throw new ArgumentNullException(nameof(fileSelectionService));
            }

            var filesToDelete = fileSelectionService.GetFiles();

            // Rest of the code...

        }
    }
}