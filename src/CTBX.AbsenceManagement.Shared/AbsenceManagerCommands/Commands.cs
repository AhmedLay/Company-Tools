namespace CTBX.AbsenceManagement.Shared.AbsenceManagerCommands;

public record SchedulingVacation(
    string Id,
    int EmployeeId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Comment,
    DateTimeOffset ScheduledAt
);

public record RequestingSickLeave(
    string Id,
    int EmployeeId,
    DateTimeOffset From,
    DateTimeOffset Until,
    DateTimeOffset ReportedAt
);

public record ChangingVacationSchedule(
    string Id,
    int EmployeeId,
    int SupervisorId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Comment,
    DateTimeOffset ScheduledAt
);

public record ApprovingVacation(
    string Id,
    int SupervisorId,
    DateTimeOffset ApprovedAt
);

public record RejectingRequest(
    string Id,
    int SupervisorId,
    int EmployeeId,
    DateTimeOffset RejectedAt,
    string Reason
);

public record AbdoningRequest(
    string Id,
    int EmployeeId,
    DateTimeOffset ApprovedAt,
    string Reason
);

