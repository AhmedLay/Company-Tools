using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventuous;

namespace MinimalApiArchitecture.Application
{
    public record AbsenceState : State<AbsenceState>
    {
        public string EmployeeId { get; set; } = string.Empty;
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }
    
}
