using ArchivedSalesFileCleaner.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ArchivedSalesFileCleaner.Services
{
    public class FileSelectionService
    {
        private readonly string directoryPath;
        private readonly int retentionPeriodInDays;
        private readonly bool removeFilePhysically;
        private readonly FileOperationType configuredFileOperationType;
        private readonly ILogger<FileSelectionService> logger;

        public FileSelectionService(IOptions<DeleteSettings> settings, ILogger<FileSelectionService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

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

            logger.LogDebug("FileSelectionService created with settings: {@DeleteSettings}", settings.Value);
        }

        public List<FileDeletionRequest> GetFiles()
        {
            logger.LogInformation("Getting files from directory: {DirectoryPath}", directoryPath);

            var cutoffDate = DateTime.Now.AddDays(-retentionPeriodInDays);

            var filesToDelete = Directory.GetFiles(directoryPath, "*.zip")
                .Where(file => IsOlderThanCutoffDate(file, cutoffDate))
                .ToList();
            List<FileDeletionRequest> requestList = new();

            foreach (var file in filesToDelete)
            {
                logger.LogInformation("File {FilePath} is marked for deletion", file);

                requestList.Add(new FileDeletionRequest
                {
                    fileOperationType = configuredFileOperationType,
                    filePath = file,
                });
            }

            logger.LogInformation("{Count} files found for deletion in directory: {DirectoryPath}", requestList.Count, directoryPath);

            return requestList;
        }

        private bool IsOlderThanCutoffDate(string filePath, DateTime cutoffDate)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath).Replace(" ", string.Empty);

            if (!DateTime.TryParseExact(fileName, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fileDate))
            {
                logger.LogWarning("Could not parse date from file name: {FileName}", fileName);
                return false;
            }

            var isOlder = fileDate < cutoffDate;

            if (isOlder)
            {
                logger.LogInformation("File {FilePath} is older than the retention period and will be deleted", filePath);
            }

            return isOlder;
        }
    }
}
