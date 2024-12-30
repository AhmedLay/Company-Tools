using MinimalApiArchitecture.Application;
using Eventuous.Projections.MongoDB;
using MongoDB.Driver;
using Eventuous.Subscriptions.Context;
using Eventuous.Postgresql.Projections;
using Eventuous;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Reflection.Metadata;
using System;

public class AbsenceManagementProjection : PostgresProjector
{
    readonly ILogger<AbsenceManagementProjection> _logger;
    public AbsenceManagementProjection(NpgsqlDataSource dataSource, ILogger<AbsenceManagementProjection> logger, ITypeMapper? mapper = null) : base(dataSource, mapper)
    {
        _logger = logger;
        On<VacationScheduled>(Handle);
    }

    NpgsqlCommand Handle(NpgsqlConnection connection, MessageConsumeContext<VacationScheduled> consumeContext)
    {
        var sql = @"
        INSERT INTO absencemanagement.vacationrequests(
        id,
        employeeid,
        from,
        to,
        comment,
        scheduledat)
        VALUES(
        @id,
        @employeeid,
        @from,
        @to,
        @comment,
        @scheduledat);";

        var @event = consumeContext.Message;
        NpgsqlParameter[] parameters =
        {
            new NpgsqlParameter("id",consumeContext.Stream.GetId()),
            new NpgsqlParameter("employeeid", @event.EmployeeID),
            new NpgsqlParameter("from", @event.From),
            new NpgsqlParameter("to", @event.To),
            new NpgsqlParameter("comment", @event.Comment),
            new NpgsqlParameter("scheduledat", @event.ScheduledAt),
        };
        _logger.LogInformation("Projection handler registered SQL: {0}", sql);
        return Project(connection, sql, parameters);
    }
}



