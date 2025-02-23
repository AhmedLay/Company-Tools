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
        public int EmployeeId { get; init; }
        public bool VacationApproved { get; private init; }
        public bool SickLeaveConfirmed { get; private init; }

        public AbsenceState()
        {
            On<VacationApproved>((state, evt) => state with { VacationApproved = true });
            On<SickLeaveConfirmed>((state, evt) => state with { SickLeaveConfirmed = true });
        }
    }
}
