namespace CTBX.AbsenceManagement.Shared.AbsenceManagerCommands;

public record RequestSickLeave(
    string Id,
    int EmployeeId,
    DateTimeOffset From,
    DateTimeOffset Until,
    DateTimeOffset ReportedAt
    );

