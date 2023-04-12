using ArchivedSalesFileCleaner.Shared;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Globalization;

namespace ArchivedSalesFileCleaner.Services
{
    public class FileSelectionService
    {
        private readonly string directoryPath;
        private readonly int retentionPeriodInDays;
        private readonly bool removeFilePhysically;
        private readonly FileOperationType configuredFileOperationType;

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

          

            directoryPath = settings.Value.SourceDirectory;
            retentionPeriodInDays = settings.Value.RetentionPeriodInDays;
            removeFilePhysically = settings.Value.RemoveFilePhysically;

            configuredFileOperationType = removeFilePhysically ? FileOperationType.Delete : FileOperationType.Archive;
        }

        public List<FileDeletionRequest> GetFiles()
        {
            var cutoffDate = DateTime.Now.AddDays(-retentionPeriodInDays);

            var filesToDelete = Directory.GetFiles(directoryPath, "*.zip")
                .Where(file => IsOlderThanCutoffDate(file, cutoffDate))
                .ToList();
            List<FileDeletionRequest> requestList = new();

            foreach (var file in filesToDelete)
            {
                requestList.Add(new FileDeletionRequest
                {                   
                    fileOperationType = configuredFileOperationType,
                    filePath = file,
                });
            }

            return requestList;
        }

        private bool IsOlderThanCutoffDate(string filePath, DateTime cutoffDate)
        {
             var fileName = Path.GetFileNameWithoutExtension(filePath).Replace(" ",string.Empty);

            if (!DateTime.TryParseExact(fileName, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fileDate))
            {
                return false;
            }

            return fileDate < cutoffDate;
        }
    }

}