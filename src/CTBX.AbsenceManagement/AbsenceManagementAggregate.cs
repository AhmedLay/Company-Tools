﻿using Eventuous;

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

        public void ChangeSchedule(int id, DateTimeOffset from, DateTimeOffset to, string comment)
        {
            EnsureExists();
            Apply(
                new VacationChanged
                {
                    EmployeeID = id,
                    From = from,
                    To = to,
                    Comment = comment,
                    ChangedAt = DateTimeOffset.UtcNow
                });
        }

        //public void RequestVacation(int employeeid, int supervisorid, DateTimeOffset from, DateTimeOffset to, string comment, DateTimeOffset scheduledat)
        //{
        //    EnsureExists();

        //    Apply(
        //        new VacationRequest
        //        {
        //            EmployeeID = employeeid,
        //            SupervisorID = supervisorid,
        //            From = from,
        //            To = to,
        //            Comment = comment,
        //            ScheduledAt = scheduledat
        //        });
        //}

        //public void ApproveRequest(int supervisorid, DateTimeOffset approvedat)
        //{
        //    EnsureExists();

        //    Apply(
        //        new VacationApproved
        //        {   
        //            SupervisorID = supervisorid,
        //            ApprovedAt = approvedat
        //        });
        //}

        //public void RejectRequest(int supervisorid, int employeeid,DateTimeOffset rejectedat, string reason)
        //{
        //    EnsureExists();

        //    Apply(
        //        new VacationRejected
        //        {
        //            SupervisorID = supervisorid,
        //            EmployeeID = employeeid,
        //            RejectedAt = rejectedat,
        //            Reason = reason
        //        });
        //}

        //public void AbondonRequest(int employeeid, DateTimeOffset approvedat, string reason)
        //{
        //    EnsureExists();

        //    Apply(
        //        new VacationAbondon
        //        {
        //            EmployeeID = employeeid,
        //            Reason = reason,
        //            ApprovedAt = approvedat
        //        });
        //}

    }


    [EventType("V1.VacationScheduled")]
    public record VacationScheduled
    {
        public int EmployeeID { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset ScheduledAt { get; set; }
    }

    [EventType("V1.Changed")]
    public record VacationChanged
    {
        public int EmployeeID { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset ChangedAt { get; set; }
    }


    //[EventType("V1.VacationRequest")]
    //public record VacationRequest
    //{
    //    public int EmployeeID { get; set; } 
    //    public int SupervisorID { get; set; }
    //    public DateTimeOffset From { get; set; }
    //    public DateTimeOffset To { get; set; }
    //    public string Comment { get; set; } = string.Empty;
    //    public DateTimeOffset ScheduledAt { get; set; }
    //}
    //[EventType("V1.VacationApproved")]
    //public record VacationApproved
    //{
    //    public int SupervisorID { get; set; }
    //    public DateTimeOffset ApprovedAt { get; set; }

    //}
    //[EventType("V1.VacationRejected")]
    //public record VacationRejected
    //{
    //    public int SupervisorID { get; set; }
    //    public int EmployeeID { get; set; }
    //    public DateTimeOffset RejectedAt { get; set; }
    //    public string Reason { get; set; } = string.Empty;

    //}
    //[EventType("V1.VacationAbondon")]
    //public record VacationAbondon
    //{
    //    public int EmployeeID { get; set; }
    //    public DateTimeOffset ApprovedAt { get; set; }
    //    public string Reason { get; set; } = string.Empty;

    //}
}
