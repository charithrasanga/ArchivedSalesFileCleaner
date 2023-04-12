using ArchivedSalesFileCleaner.Shared;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchivedSalesFileCleaner.Services
{

    public class FileOperationService
    {
        private readonly string archivePath;

        public FileOperationService(IOptions<DeleteSettings> settings)
        {
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

                    throw new ArgumentException($"Failed to create directory. '{(settings.Value.SourceDirectory)}' is not a valid directory. Make sure directory exist in your file system or ensure program is authorized to create the directory.\n{ex.Message}", nameof(settings));
                }

            }

            archivePath = settings.Value.ArchivePath;
        }

        public List<FileDeletionResponse> ProcessFiles(List<FileDeletionRequest> fileDeletionRequests)
        {
            List<FileDeletionResponse> responseList = new();

            foreach (var file in fileDeletionRequests)
            {
                try
                {
                    if (file.fileOperationType == FileOperationType.Delete)
                    {
                       
                            File.Delete(file.filePath);
 
                    }
                    else
                    {
                        File.Move(file.filePath, Path.Combine(archivePath, Path.GetFileName(file.filePath)));
                    }

                    responseList.Add(new FileDeletionResponse
                    {
                        filePath = file.filePath,
                        fileOperationType = file.fileOperationType,
                        fileOperationStatus = FileOperationStatus.Success
                    });
                }
                catch (Exception ex)
                {
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


