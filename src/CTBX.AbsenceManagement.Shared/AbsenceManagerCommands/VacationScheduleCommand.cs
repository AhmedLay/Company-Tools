using MongoDB.Bson.Serialization.Attributes;
namespace MinimalApiArchitecture.Application.Commands
{
    public record VacationScheduleCommand(
    string Id,
    int EmployeeId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Comment,
    DateTimeOffset ScheduledAt);
}

