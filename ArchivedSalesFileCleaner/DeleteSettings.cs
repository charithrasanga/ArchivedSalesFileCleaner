using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArchivedSalesFileCleaner
{
    public partial class DeleteSettings
    {
        public int RetentionPeriodInDays { get; set; }
        public string SourceDirectory { get; set; }
        public bool RemoveFilePhysically { get; set; }
        public string ArchivePath { get; set; }
    }
}
