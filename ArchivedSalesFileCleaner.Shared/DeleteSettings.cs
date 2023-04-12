namespace ArchivedSalesFileCleaner.Shared
{
    public class DeleteSettings
    {
        public int RetentionPeriodInDays { get; set; }
        public string SourceDirectory { get; set; }
        public bool RemoveFilePhysically { get; set; }
        public string ArchivePath { get; set; }
    }
}