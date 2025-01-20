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
        public string Title { get; set; } = string.Empty;
    }
}
