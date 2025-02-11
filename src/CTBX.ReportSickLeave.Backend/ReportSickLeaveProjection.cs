using Microsoft.Extensions.Logging;
using Eventuous.Postgresql.Projections;
using Npgsql;
using Eventuous;

using CTBX.ReportSickLeave.Shared;
using Eventuous.Subscriptions.Context;
using CTBX.ReportSickLeave.Backend.Events;

namespace CTBX.ReportSickLeave.Backend;

public class ReportSickLeaveProjection : PostgresProjector
{
    readonly ILogger<ReportSickLeaveProjection> _logger;

    public ReportSickLeaveProjection(NpgsqlDataSource dataSource, ILogger<ReportSickLeaveProjection> logger, ITypeMapper? mapper = null) : base(dataSource, mapper)
    {
        _logger = logger;
        On<SickLeaveRequested>(HandleSickLeaveRequested);
        On<SickLeaveApproved>(HandleSickLeaveApproved);
        On<SickLeaveRejected>(HandleSickLeaveRejected);
        On<SickLeaveDeleted>(HandleSickLeaveDeleted);
    }

    private NpgsqlCommand HandleSickLeaveRequested(NpgsqlConnection connection, MessageConsumeContext<SickLeaveRequested> context)
    {
        var sql = @"
            INSERT INTO sick_leave_reports (
                id,
                employee_id,
                from_date,
                until_date,
                status,
                reported_at,
                approved_at,
                rejected_at
            ) VALUES (
                @id,
                @employeeId,
                @from,
                @until,
                @status,
                @reportedAt,
                @approvedAt,
                @rejectedAt
            );";

        var @event = context.Message;

        NpgsqlParameter[] parameters =
        {
            new NpgsqlParameter("id", context.Stream.GetId()),
            new NpgsqlParameter("employeeId", @event.EmployeeId),
            new NpgsqlParameter("from", @event.From),
            new NpgsqlParameter("until", @event.Until),
            new NpgsqlParameter("status", SickLeaveStatus.Requested.ToString()),
            new NpgsqlParameter("reportedAt", @event.ReportedAt),
            new NpgsqlParameter("approvedAt", DBNull.Value), 
            new NpgsqlParameter("rejectedAt", DBNull.Value)  
        };

        _logger.LogInformation("Projection handler registered SQL: {0}", sql);

        return Project(connection, sql, parameters);
    }

    private NpgsqlCommand HandleSickLeaveApproved(NpgsqlConnection connection, MessageConsumeContext<SickLeaveApproved> context)
    {
        var sql = @"
            UPDATE sick_leave_reports
            SET
                status = @status,
                approved_at = @approvedAt,
                rejected_at = @rejectedAt
            WHERE id = @id;";

        var @event = context.Message;

        NpgsqlParameter[] parameters =
        {
            new NpgsqlParameter("id", context.Stream.GetId()),
            new NpgsqlParameter("status", SickLeaveStatus.Approved.ToString()),
            new NpgsqlParameter("approvedAt", @event.ApprovedAt),
            new NpgsqlParameter("rejectedAt", DBNull.Value) // Reset rejected_at if it was previously set
        };

        _logger.LogInformation("Projection handler registered SQL: {0}", sql);

        return Project(connection, sql, parameters);
    }

    private NpgsqlCommand HandleSickLeaveRejected(NpgsqlConnection connection, MessageConsumeContext<SickLeaveRejected> context)
    {
        var sql = @"
            UPDATE sick_leave_reports
            SET
                status = @status,
                rejected_at = @rejectedAt,
                approved_at = @approvedAt
            WHERE id = @id;";

        var @event = context.Message;

        NpgsqlParameter[] parameters =
        {
            new NpgsqlParameter("id", context.Stream.GetId()),
            new NpgsqlParameter("status", SickLeaveStatus.Rejected.ToString()),
            new NpgsqlParameter("rejectedAt", @event.RejectedAt),
            new NpgsqlParameter("approvedAt", DBNull.Value) // Reset approved_at if it was previously set
        };

        _logger.LogInformation("Projection handler registered SQL: {0}", sql);

        return Project(connection, sql, parameters);
    }

    private NpgsqlCommand HandleSickLeaveDeleted(NpgsqlConnection connection, MessageConsumeContext<SickLeaveDeleted> context)
    {
        var sql = @"
            DELETE FROM sick_leave_reports
            WHERE id = @id;";

        var @event = context.Message;

        NpgsqlParameter[] parameters =
        {
            new NpgsqlParameter("id", context.Stream.GetId())
        };

        _logger.LogInformation("Projection handler registered SQL: {0}", sql);

        return Project(connection, sql, parameters);
    }

}
