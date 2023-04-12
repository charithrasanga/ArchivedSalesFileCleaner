using ArchivedSalesFileCleaner.Shared;
using Microsoft.Extensions.Options;
using System.Configuration;

namespace ArchivedSalesFileCleaner.Services
{
    public class FileSelectionService
    {
        private readonly string directoryPath;
        private readonly int retentionPeriodInDays;
        private readonly bool removeFilePhysically;

        public FileSelectionService(IOptions<DeleteSettings> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(settings.Value.SourceDirectory))
            {
                throw new ArgumentException($"'{nameof(settings.Value.SourceDirectory)}' cannot be null or whitespace.", nameof(settings));
            }

            if (!Directory.Exists(settings.Value.SourceDirectory))
            {
                throw new ArgumentException($"Directory '{(settings.Value.SourceDirectory)}' is not a valid directory make sure directory exist in your file system.", nameof(settings));
            }

            directoryPath = settings.Value.SourceDirectory;
            retentionPeriodInDays = settings.Value.RetentionPeriodInDays;
            removeFilePhysically = settings.Value.RemoveFilePhysically;
        }

        public List<string> GetFiles()
        {
            var cutoffDate = DateTime.Now.AddDays(-retentionPeriodInDays);

            var filesToDelete = Directory.GetFiles(directoryPath, "*.zip")
                .Where(file => IsOlderThanCutoffDate(file, cutoffDate))
                .ToList();

            return filesToDelete;
        }

        private bool IsOlderThanCutoffDate(string filePath, DateTime cutoffDate)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            if (!DateTime.TryParse(fileName, out DateTime fileDate))
            {
                return false;
            }

            return fileDate < cutoffDate;
        }
    }

    public class FileOperationService
    {

    }
}