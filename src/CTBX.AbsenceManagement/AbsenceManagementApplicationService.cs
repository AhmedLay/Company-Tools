using CTBX.AbsenceManagement.Shared;
using Eventuous;

namespace MinimalApiArchitecture.Application
{
    public class AbsenceManagementApplicationService : CommandService<AbsenceManagementAggregate, AbsenceState, AbsenceId>
    {
        public AbsenceManagementApplicationService(IEventStore store) : base(store)
        {
            On<SchedulingVacation>()
            .InState(ExpectedState.New)
            .GetId(cmd => new AbsenceId(cmd.Id))
            .Act((aggregate, cmd) => aggregate.ScheduleVacation(cmd.EmployeeId, cmd.From, cmd.To, cmd.Comment, cmd.ScheduledAt));

            On<ChangingVacationSchedule>()
                .InState(ExpectedState.Existing)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.EditVacationRequest(cmd.EmployeeId,cmd.From, cmd.To, cmd.Comment, cmd.ScheduledAt));

            On<RequestingVacation>()
                .InState(ExpectedState.Existing)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.RequestVacation(cmd.SupervisorId, cmd.Requestedat));

            On<ApprovingVacation>()
                .InState(ExpectedState.Existing)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.ApproveRequest(cmd.SupervisorId, cmd.ApprovedAt));

            On<RejectingRequest>()
                .InState(ExpectedState.Existing)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.RejectRequest(cmd.SupervisorId, cmd.RejectedAt));

            On<AbdoningRequest>()
                .InState(ExpectedState.Existing)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.AbandonRequest(cmd.AbondonAt));

            On<RequestingSickLeave>()
                .InState(ExpectedState.New)
                .GetId(cmd => new AbsenceId(cmd.Id))
                .Act((aggregate, cmd) => aggregate.RequestSickLeave(cmd.EmployeeId, cmd.From, cmd.Until, cmd.ReportedAt, cmd.Comment));
        }
    }
    public record AbsenceId(string Value) : Id(Value);
}
