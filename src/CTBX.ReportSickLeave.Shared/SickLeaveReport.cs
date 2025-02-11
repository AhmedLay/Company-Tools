
namespace CTBX.ReportSickLeave.Shared;

public class SickLeaveReport
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime From { get; set; }
    public DateTime Until { get; set; }
    public SickLeaveStatus Status { get; set; }
    public DateTime ReportedAt { get; set; }
    public DateTime ApprovedAt { get; set; }
    public DateTime RejectedAt { get; set; }

    //public string Document { get; set; } = string.Empty;
    //public int ReportedBy { get; set; }
}

public enum SickLeaveStatus
{
    Requested,
    Approved,
    Rejected
}
