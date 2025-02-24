using CTBX.AbsenceManagement.Shared.AbsenceManagerCommands;
using Eventuous;

namespace MinimalApiArchitecture.Application
{
    public class AbsenceManagementApplicationService : CommandService<AbsenceManagementAggregate, AbsenceState, AbsenceId>
    {
        public AbsenceManagementApplicationService(IEventStore store) : base(store)
        {
            On<CTBX.AbsenceManagement.Shared.AbsenceManagerCommands.SchedulingVacation>()
            .InState(ExpectedState.New)
            .GetId(cmd => new AbsenceId(cmd.Id))
            .Act((aggregate, cmd) => aggregate.ScheduleVacation(cmd.EmployeeId, cmd.From, cmd.To, cmd.Comment, cmd.ScheduledAt));

            On<RequestingSickLeave>()
                .InState(ExpectedState.New)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.RequestSickLeave(cmd.EmployeeId, cmd.From, cmd.Until, cmd.ReportedAt));

            On<ChangingVacationSchedule>()
                .InState(ExpectedState.Existing)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.RequestVacation(cmd.EmployeeId, cmd.SupervisorId, cmd.From, cmd.To, cmd.Comment, cmd.ScheduledAt));

            On<ApprovingVacation>()
                .InState(ExpectedState.Existing)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.ApproveRequest(cmd.SupervisorId, cmd.ApprovedAt));

            On<RejectingRequest>()
                .InState(ExpectedState.Existing)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.RejectRequest(cmd.SupervisorId, cmd.EmployeeId, cmd.RejectedAt, cmd.Reason));

            On<AbdoningRequest>()
                .InState(ExpectedState.Existing)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.AbandonRequest(cmd.EmployeeId, cmd.ApprovedAt, cmd.Reason));
        }
    }

    public record AbsenceId(string Value) : Id(Value);
}
