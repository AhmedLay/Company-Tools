//using CTBX.ReportSickLeave.Shared;
//using Eventuous;
//using static CTBX.ReportSickLeave.Backend.ReportSickLeaveCommands;
//using static CTBX.ReportSickLeave.Backend.Events;
//using CTBX.ReportSickLeave.Backend.Events;



//namespace CTBX.ReportSickLeave.Backend;

//public class ReportSickLeaveAggregate
//{
//    public class SickLeave : Aggregate<SickLeaveState>
//    {
//        public void Request(RequestSickLeave command)
//        {
//            EnsureDoesntExist();

//            Apply(new SickLeaveRequested(
//                command.EmployeeId,
//                command.From,
//                command.Until,
//                command.Status,
//                command.ReportedAt));
//        }
//        public void Approve(ApproveSickLeave command)
//        {
//            if (State.Status != SickLeaveStatus.Requested)
//            {
//                throw new InvalidOperationException("Sick leave must be in requested status to be approved.");
//            }
//            Apply(new SickLeaveApproved(command.Id, command.Status, DateTime.UtcNow));
//        }

//        public void Reject(RejectSickLeave command)
//        {
//            if (State.Status != SickLeaveStatus.Requested)
//            {
//                throw new InvalidOperationException("Sick leave must be in requested status to be rejected.");
//            }
//            Apply(new SickLeaveRejected(command.Id, command.Status, DateTime.UtcNow));
//        }

//        public void Delete(DeleteSickLeave command)
//        {
//            Apply(new SickLeaveDeleted(command.Id, DateTime.UtcNow));
//        }

//        }
//}


