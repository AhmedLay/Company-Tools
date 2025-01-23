using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTBX.ReportSickLeave.Shared
{
    public class SickLeaveReport
    {
        public int EmployeeId { get; set; }
        public DateTime From { get; set; }
        public DateTime Until { get; set; }
        public DateTime ReportedAt { get; set; }
        public string Document { get; set; } = string.Empty;
        public int ReportedBy { get; set; }
    }
}
