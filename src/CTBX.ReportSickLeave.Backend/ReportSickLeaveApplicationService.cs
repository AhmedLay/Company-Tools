//using System.Security.Cryptography;
//using Eventuous;
//using static CTBX.ReportSickLeave.Backend.ReportSickLeaveCommands;


//namespace CTBX.ReportSickLeave.Backend;
//public class ReportSickLeaveApplicationService : CommandService<ReportSickLeaveAggregate.SickLeave, SickLeaveState, SickLeaveId>
//{
//    public ReportSickLeaveApplicationService(IEventStore store) : base(store)
//    {
//        On<RequestSickLeave>()
//            .InState(ExpectedState.New)
//            .GetId(cmd => new SickLeaveId(cmd.EmployeeId.ToString()))
//            .Act((aggregate, cmd) =>
//            {
//                aggregate.Request(cmd.EmployeeId, cmd.From, cmd.Until, cmd.Status, cmd.ReportedAt);
//            });


//        On<ApproveSickLeave>()
//            .InState(ExpectedState.Existing)
//            .GetId(cmd => new SickLeaveId(cmd.Id.ToString()))
//            .Act((aggregate, cmd) =>
//            {
//                aggregate.Approve(cmd.Id, cmd.Status);
//            });

//        On<RejectSickLeave>()
//            .InState(ExpectedState.Existing)
//            .GetId(cmd => new SickLeaveId(cmd.Id.ToString()))
//            .Act((aggregate, cmd) =>
//            {
//                aggregate.Reject(cmd.Id, cmd.Status);
//            });

//        On<DeleteSickLeave>()
//            .InState(ExpectedState.Existing)
//            .GetId(cmd => new SickLeaveId(cmd.Id.ToString()))
//            .Act((aggregate, cmd) =>
//            {
//                aggregate.Delete(cmd.Id);
//            });

//    }


    

//}
