using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTBX.EmployeesImport.UI.Models
{
    public class SaveFile
    {
        public SaveFile()
        {
            Files = new List<FileData>();
        }
        public List<FileData> Files { get; set; }
    }


    public class FileData
    {
        public byte[] ImagesBytes { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }

    }

}
