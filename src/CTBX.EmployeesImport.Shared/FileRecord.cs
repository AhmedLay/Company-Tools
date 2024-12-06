using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTBX.EmployeesImport.Shared
{
    public class FileRecord
    {
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileStatus { get; set; } = string.Empty;
    }
}
