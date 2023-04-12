namespace ArchivedSalesFileCleaner.Shared
{
    public class FileDeleteRequest
    {
        public string FilePath { get; set; }
        public FileOperationType fileOperationType { get; set; }
        public string RemoveFilePhysically { get; set; }
        public string ArchivePath { get; set; }
    }
}