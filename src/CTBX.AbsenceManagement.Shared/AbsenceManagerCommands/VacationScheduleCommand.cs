
namespace MinimalApiArchitecture.Application.Commands
{
    public record VacationScheduleCommand(string Id, int EmployeeID, DateTimeOffset From, DateTimeOffset To, string Comment,  DateTimeOffset ScheduledAt);
}
