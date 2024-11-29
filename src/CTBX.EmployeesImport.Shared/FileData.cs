using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTBX.EmployeesImport.Shared;

public class FileData
{
    public string? FileName { get; set; }
    public byte[]? FileContent { get; set; }
    public string Id { get; set; } = "";
}

