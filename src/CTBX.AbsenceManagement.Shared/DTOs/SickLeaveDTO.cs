using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTBX.AbsenceManagement.Shared.DTOs;

public class SickLeaveDTO
{
    public string Id { get; set; } = string.Empty;
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }
    public string? Comment { get; set; } = string.Empty;
    public DateTimeOffset ReportedAt { get; set; }
}

