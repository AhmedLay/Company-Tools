using Eventuous;

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

            On<VacationChangeCommand>()
                .InState(ExpectedState.Existing)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.ChangeSchedule(cmd.EmployeeId, cmd.From, cmd.To, cmd.Comment));
        }

    }

    public record AbsenceId(string Value) : Id(Value);
}
