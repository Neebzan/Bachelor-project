using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class FileDataModel
    {
        public List<FileModel> Files { get; set; } = new List<FileModel>();
        public long TotalSize { get; set; }
        public long RemainingSize { get; set; }
    }

    public class FileModel
    {
        public string FilePath { get; set; }
        public long Size { get; set; }
    }
}
