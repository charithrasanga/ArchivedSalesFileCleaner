using System.Configuration;

namespace ArchivedSalesFileCleaner.Services
{
    public class FileSelectionService
    {
        private readonly string directoryPath;

        public FileSelectionService(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentException($"'{nameof(directoryPath)}' cannot be null or whitespace.", nameof(directoryPath));
            }

            if (!Directory.Exists(directoryPath))
            {
                throw new ArgumentException($"Directory '{(directoryPath)}' is not a valid direcotry name.", nameof(directoryPath));
            }

            this.directoryPath = directoryPath;
        }

        public List<string> GetFiles(int retentionPeriod)
        {
            List<string> oldFiles = new List<string>();
            DateTime cutoffDate = DateTime.Now.AddDays(-retentionPeriod);

            try
            {
                string retentionPeriodString = ConfigurationManager.AppSettings["RetentionPeriodInDays"];
                if (!int.TryParse(retentionPeriodString, out retentionPeriod))
                {
                    retentionPeriod = 30; // Default to 30 days if retention period is not specified or invalid
                }

                DirectoryInfo directory = new DirectoryInfo(directoryPath);
                FileInfo[] files = directory.GetFiles("*.zip");

                foreach (FileInfo file in files)
                {
                    DateTime fileDate;
                    if (DateTime.TryParseExact(file.Name.Substring(0, 10), "yyyy-MMM-dd", null, System.Globalization.DateTimeStyles.None, out fileDate))
                    {
                        if (fileDate < cutoffDate)
                        {
                            oldFiles.Add(file.FullName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur when getting the list of files
                Console.WriteLine($"Error getting list of files: {ex.Message}");
            }

            return oldFiles;
        }
    }

    public class FileOperationService
    {

    }
}