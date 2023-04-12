namespace ArchivedSalesFileCleaner.Shared
{
    public class FileDeletionRequest
    {
        public string filePath { get; set; }
        public FileOperationType fileOperationType { get; set; }

    }

    public class FileDeletionResponse
    {
        public string filePath { get; set; }
        public string errorMessage { get; set; }=string.Empty;
        public FileOperationType fileOperationType { get; set; }
        public FileOperationStatus fileOperationStatus { get; set; }
    }
}