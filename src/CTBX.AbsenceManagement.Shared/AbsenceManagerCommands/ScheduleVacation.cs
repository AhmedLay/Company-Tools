namespace MinimalApiArchitecture.Application.Commands;

public record ScheduleVacation(
string Id,
int EmployeeId,
DateTimeOffset From,
DateTimeOffset To,
string Comment,
DateTimeOffset ScheduledAt);

