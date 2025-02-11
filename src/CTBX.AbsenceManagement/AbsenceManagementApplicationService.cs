using CTBX.AbsenceManagement.Shared.AbsenceManagerCommands;
using Eventuous;
using MinimalApiArchitecture.Application.Commands;

namespace MinimalApiArchitecture.Application
{
    public class AbsenceManagementApplicationService : CommandService<AbsenceManagementAggregate, AbsenceState, AbsenceId>
    {
        public AbsenceManagementApplicationService(IEventStore store) : base(store)
        {
            On<VacationScheduleCommand>()
                .InState(ExpectedState.New)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.ScheduleVacation(cmd.EmployeeId, cmd.From, cmd.To, cmd.Comment, cmd.ScheduledAt));

            //
            On<RequestSickLeave>()
                .InState(ExpectedState.New)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.RequestSickLeave(cmd.EmployeeId, cmd.From, cmd.Until, cmd.ReportedAt));


        }
    }

    public record AbsenceId(string Value) : Id(Value);
}
