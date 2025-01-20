using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTBX.AbsenceManagement.Shared.DTOs
{
    public class VacationScheduleDTO
    {
        public int EmployeeID { get; set; } 
        public string Id { get; set; } = string.Empty;
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public string Comment { get; set; } = string.Empty;

    }
}
