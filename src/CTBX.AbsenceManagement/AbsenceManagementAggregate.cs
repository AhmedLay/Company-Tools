using Eventuous;

namespace MinimalApiArchitecture.Application
{
    public class AbsenceManagementAggregate : Aggregate<AbsenceState>
    {
        public void ScheduleVacation(int id,DateTimeOffset from, DateTimeOffset to, string comment, DateTimeOffset scheduledat)
        {
            EnsureDoesntExist();
            Apply(
                new VacationScheduled
                {
                    EmployeeID = id,
                    From = from,
                    To = to,
                    Comment = comment,
                    ScheduledAt = scheduledat
                });
        }

        public void RequestVacation(int employeeid, int supervisorid, DateTimeOffset from, DateTimeOffset to, string comment, DateTimeOffset scheduledat)
        {
            EnsureExists();

            Apply(
                new VacationRequest
                {
                    EmployeeID = employeeid,
                    SupervisorID = supervisorid,
                    From = from,
                    To = to,
                    Comment = comment,
                    ScheduledAt = scheduledat
                });
        }

        public void ApproveRequest(int supervisorid, DateTimeOffset approvedat)
        {
            EnsureExists();

            Apply(
                new VacationApproved
                {   
                    SupervisorID = supervisorid,
                    ApprovedAt = approvedat
                });
        }

        public void RejectRequest(int supervisorid, int employeeid,DateTimeOffset rejectedat, string reason)
        {
            EnsureExists();

            Apply(
                new VacationRejected
                {
                    SupervisorID = supervisorid,
                    EmployeeID = employeeid,
                    RejectedAt = rejectedat,
                    Reason = reason
                });
        }

        public void AbondonRequest(int employeeid, DateTimeOffset approvedat, string reason)
        {
            EnsureExists();

            Apply(
                new VacationAbondon
                {
                    EmployeeID = employeeid,
                    Reason = reason,
                    ApprovedAt = approvedat
                });
        }

    }



    public record VacationScheduled
    {
        public int EmployeeID { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset ScheduledAt { get; set; }
    }

    public record VacationRequest
    {
        public int EmployeeID { get; set; } 
        public int SupervisorID { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset ScheduledAt { get; set; }
    }

    public record VacationApproved
    {
        public int SupervisorID { get; set; }
        public DateTimeOffset ApprovedAt { get; set; }

    }

    public record VacationRejected
    {
        public int SupervisorID { get; set; }
        public int EmployeeID { get; set; }
        public DateTimeOffset RejectedAt { get; set; }
        public string Reason { get; set; } = string.Empty;

    }

    public record VacationAbondon
    {
        public int EmployeeID { get; set; }
        public DateTimeOffset ApprovedAt { get; set; }
        public string Reason { get; set; } = string.Empty;

    }
}
