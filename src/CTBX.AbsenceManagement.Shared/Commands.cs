namespace CTBX.AbsenceManagement.Shared;

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
    string Comment,
    DateTimeOffset ReportedAt
);

public record ChangingVacationSchedule(
    string Id,
    int EmployeeId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Comment,
    DateTimeOffset ScheduledAt
);

public record RequestingVacation(
    string Id,
    int SupervisorId,
    DateTimeOffset Requestedat
 );

public record ApprovingVacation(
    string Id,
    int SupervisorId,
    DateTimeOffset ApprovedAt
);

public record RejectingRequest(
    string Id,
    int SupervisorId,
    DateTimeOffset RejectedAt
    //string Reason
);

public record AbdoningRequest(
    string Id,
    DateTimeOffset AbondonAt
);

