using CTBX.AbsenceManagement.Shared;
using Eventuous;

namespace MinimalApiArchitecture.Application
{
    public class AbsenceManagementAggregate : Aggregate<AbsenceState>
    {
        public void ScheduleVacation(int id, DateTimeOffset from, DateTimeOffset to, string comment, DateTimeOffset scheduledAt)
        {
            EnsureDoesntExist();
            Apply(new VacationSchedule
            {
                EmployeeID = id,
                From = from,
                To = to,
                Comment = comment,
                ScheduledAt = scheduledAt,
                Status = Status.Drafted  
            });
        }

        public void RequestSickLeave(int employeeId, DateTimeOffset from, DateTimeOffset until, DateTimeOffset reportedAt,string comment)
        {
            EnsureDoesntExist();
            Apply(new SickLeaveRequested
            {
                EmployeeId = employeeId,
                From = from,
                Until = until,
                Comment = comment,
                ReportedAt = reportedAt
            });
        }

        public void EditVacationRequest(int employeeId, DateTimeOffset from, DateTimeOffset to, string comment, DateTimeOffset scheduledAt)
        {
            EnsureExists();
            Apply(new VacationScheduleEdit
            {
                EmployeeID = employeeId,
                From = from,
                To = to,
                Comment = comment,
                ScheduledAt = scheduledAt,
                Status = AbsenceStatus.Drafted
            });
        }

        public void RequestVacation(int employeeId, int supervisorId, DateTimeOffset from, DateTimeOffset to, string comment,DateTimeOffset requestedat)
        {
            EnsureExists();
            Apply(new VacationRequested
            {
                EmployeeID = employeeId,
                SupervisorID = supervisorId,
                From = from,
                To = to,
                Comment = comment,
                RequestedAt = requestedat,
                Status = AbsenceStatus.Requested
            });
        }

        public void ApproveRequest(int supervisorId, DateTimeOffset approvedAt)
        {
            EnsureExists();
            Apply(new VacationApproved
            {
                SupervisorID = supervisorId,
                ApprovedAt = approvedAt,
                Status = AbsenceStatus.VacationApproved
            });
        }

        public void RejectRequest(int supervisorId, int employeeId, DateTimeOffset rejectedAt, string reason)
        {
            EnsureExists();
            Apply(new VacationRejected
            {
                SupervisorID = supervisorId,
                EmployeeID = employeeId,
                RejectedAt = rejectedAt,
                Reason = reason,
                Status = AbsenceStatus.Rejected
            });
        }

        public void AbandonRequest(int employeeId, DateTimeOffset approvedAt, string reason)
        {
            EnsureExists();
            Apply(new VacationAbandoned
            {
                EmployeeID = employeeId,
                ApprovedAt = approvedAt,
                Reason = reason,
                Status = AbsenceStatus.Abondon
            });
        }

        public void ConfirmSickLeave(int employeeId, DateTimeOffset confirmedAt)
        {
            EnsureExists();
            Apply(new SickLeaveConfirmed
            {
                EmployeeId = employeeId,
                ConfirmedAt = confirmedAt
            });
        }
    }

    [EventType("V1.VacationScheduled")]
    public record VacationSchedule
    {
        public int EmployeeID { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset ScheduledAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    [EventType("V1.VacationScheduleEdit")]
    public record VacationScheduleEdit
    {
        public int EmployeeID { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset ScheduledAt { get; set; }
        public AbsenceStatus Status { get; set; }
    }

    [EventType("V1.VacationRequested")]
    public record VacationRequested
    {
        public int EmployeeID { get; set; }
        public int SupervisorID { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset RequestedAt { get; set; }
        public AbsenceStatus Status { get; set; }
    }


    [EventType("V1.VacationApproved")]
    public record VacationApproved
    {
        public int SupervisorID { get; set; }
        public DateTimeOffset ApprovedAt { get; set; }
        public AbsenceStatus Status { get; set; }
    }

    [EventType("V1.VacationRejected")]
    public record VacationRejected
    {
        public int SupervisorID { get; set; }
        public int EmployeeID { get; set; }
        public DateTimeOffset RejectedAt { get; set; }
        public string Reason { get; set; } = string.Empty;
        public AbsenceStatus Status { get; set; }
    }

    [EventType("V1.VacationAbandoned")]
    public record VacationAbandoned
    {
        public int EmployeeID { get; set; }
        public DateTimeOffset ApprovedAt { get; set; }
        public string Reason { get; set; } = string.Empty;
        public AbsenceStatus Status { get; set; }
    }

    [EventType("V1.SickLeaveRequested")]
    public record SickLeaveRequested
    {
        public int EmployeeId { get; init; }
        public DateTimeOffset From { get; init; }
        public DateTimeOffset Until { get; init; }
        public DateTimeOffset ReportedAt { get; init; }
        public string Comment { get; init; } = string.Empty;
    }

    [EventType("V1.SickLeaveConfirmed")]
    public record SickLeaveConfirmed
    {
        public int EmployeeId { get; init; }
        public DateTimeOffset ConfirmedAt { get; init; }
    }
}
