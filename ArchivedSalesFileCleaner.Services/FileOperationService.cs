using ArchivedSalesFileCleaner.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchivedSalesFileCleaner.Services
{
    public class FileOperationService
    {
        private readonly string archivePath;
        private readonly ILogger<FileOperationService> logger;

        public FileOperationService(ILogger<FileOperationService> logger, IOptions<DeleteSettings> settings)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if ((settings.Value.RemoveFilePhysically == false) && (!Directory.Exists(settings.Value.ArchivePath)))
            {
                try
                {
                    Directory.CreateDirectory(settings.Value.ArchivePath);
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Failed to create directory '{settings.Value.ArchivePath}'. Make sure directory exist in your file system or ensure program is authorized to create the directory.";
                    logger.LogError(ex, errorMessage);
                    throw new ArgumentException(errorMessage, nameof(settings));
                }
            }

            archivePath = settings.Value.ArchivePath;
        }

        public List<FileDeletionResponse> ProcessFiles(List<FileDeletionRequest> fileDeletionRequests)
        {
            var responseList = new List<FileDeletionResponse>();

            foreach (var file in fileDeletionRequests)
            {
                try
                {
                    if (file.fileOperationType == FileOperationType.Delete)
                    {
                        logger.LogInformation($"Deleting file '{file.filePath}'...");
                        File.Delete(file.filePath);
                    }
                    else
                    {
                        logger.LogInformation($"Moving file '{file.filePath}' to archive directory '{archivePath}'...");
                        File.Move(file.filePath, Path.Combine(archivePath, Path.GetFileName(file.filePath)));
                    }

                    responseList.Add(new FileDeletionResponse
                    {
                        filePath = file.filePath,
                        fileOperationType = file.fileOperationType,
                        fileOperationStatus = FileOperationStatus.Success
                    });

                    logger.LogInformation($"File operation '{file.fileOperationType}' completed successfully for file '{file.filePath}'.");

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to perform file operation '{file.fileOperationType}' on file '{file.filePath}'. Error message: {ex.Message}");
                    logger.LogError(Environment.NewLine);
                    responseList.Add(new FileDeletionResponse
                    {
                        filePath = file.filePath,
                        fileOperationType = file.fileOperationType,
                        fileOperationStatus = FileOperationStatus.Error,
                        errorMessage = ex.Message
                    });
                }
            }

            return responseList;
        }
    }
}
