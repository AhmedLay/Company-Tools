using MinimalApiArchitecture.Application;
using Eventuous.Projections.MongoDB;
using MongoDB.Driver;
using Eventuous.Subscriptions.Context;


public class AbsenceManagementProjection : MongoProjector<ReadModelDocument>
{
    public AbsenceManagementProjection(IMongoDatabase client) : base(client)
    {
        On<VacationSchedule>(aggregate => aggregate.GetId(), Handle);
        On<SickLeaveRequested>(aggregate => aggregate.GetId(), Handle);
        On<VacationScheduleEdit>(aggregate => aggregate.GetId(), Handle);
        On<VacationApproved>(aggregate => aggregate.GetId(), Handle);
        On<VacationRejected>(aggregate => aggregate.GetId(), Handle);
        On<VacationAbandoned>(aggregate => aggregate.GetId(), Handle);
        On<VacationRequested>(aggregate => aggregate.GetId(), Handle);
    }

    static UpdateDefinition<ReadModelDocument> Handle(
     IMessageConsumeContext<VacationSchedule> ctx, UpdateDefinitionBuilder<ReadModelDocument> update)
    {
        var evt = ctx.Message;

        return update.SetOnInsert(x => x.Id, ctx.Stream.GetId())
                 .Set(x => x.EmployeeId, evt.EmployeeID)
                 .Set(x => x.From, evt.From)
                 .Set(x => x.To, evt.To)
                 .Set(x => x.ScheduledAt, evt.ScheduledAt)
                 .Set(x => x.Comment, evt.Comment ?? string.Empty)
                 .Set(x => x.Status, evt.Status);
    }

    static UpdateDefinition<ReadModelDocument> Handle(
        IMessageConsumeContext<SickLeaveRequested> ctx, UpdateDefinitionBuilder<ReadModelDocument> update)
    {
        var evt = ctx.Message;
        return update.SetOnInsert(x => x.Id, ctx.Stream.GetId())
                 .Set(x => x.EmployeeId, evt.EmployeeId)
                 .Set(x => x.From, evt.From)
                 .Set(x => x.To, evt.Until)
                 .Set(x => x.ScheduledAt, evt.ReportedAt)
                 .Set(x => x.Comment, evt.Comment ?? string.Empty);
    }

    static UpdateDefinition<ReadModelDocument> Handle(
        IMessageConsumeContext<VacationScheduleEdit> ctx, UpdateDefinitionBuilder<ReadModelDocument> update)
    {
        var evt = ctx.Message;
        return update.SetOnInsert(x => x.Id, ctx.Stream.GetId())
                 .Set(x => x.EmployeeId, evt.EmployeeID)
                 .Set(x => x.From, evt.From)
                 .Set(x => x.To, evt.To)
                 .Set(x => x.ScheduledAt, evt.ScheduledAt)
                 .Set(x => x.Comment, evt.Comment ?? string.Empty) ;
    }

    static UpdateDefinition<ReadModelDocument> Handle(
       IMessageConsumeContext<VacationRequested> ctx, UpdateDefinitionBuilder<ReadModelDocument> update)
    {
        var evt = ctx.Message;
        return update.SetOnInsert(x => x.Id, ctx.Stream.GetId())
                 .Set(x => x.EmployeeId, evt.EmployeeID)
                 .Set(x => x.SupervisorId,evt.SupervisorID)
                 .Set(x => x.From, evt.From)
                 .Set(x => x.To, evt.To)
                 .Set(x => x.RequestedAt, evt.RequestedAt)
                 .Set(x => x.Comment, evt.Comment ?? string.Empty);
    }

    static UpdateDefinition<ReadModelDocument> Handle(
        IMessageConsumeContext<VacationApproved> ctx, UpdateDefinitionBuilder<ReadModelDocument> update)
    {
        var evt = ctx.Message;
        return update.SetOnInsert(x => x.Id, ctx.Stream.GetId())
                 .Set(x => x.SupervisorId, evt.SupervisorID)
                 .Set(x => x.ApprovedAt, evt.ApprovedAt);
    }

    static UpdateDefinition<ReadModelDocument> Handle(
        IMessageConsumeContext<VacationRejected> ctx, UpdateDefinitionBuilder<ReadModelDocument> update)
    {
        var evt = ctx.Message;
        return update.SetOnInsert(x => x.Id, ctx.Stream.GetId())
                 .Set(x => x.SupervisorId, evt.SupervisorID)
                 .Set(x => x.EmployeeId, evt.EmployeeID)
                 .Set(x => x.RejectedAt, evt.RejectedAt)
                 .Set(x => x.Reason, evt.Reason ?? string.Empty) ;
    }

    static UpdateDefinition<ReadModelDocument> Handle(
        IMessageConsumeContext<VacationAbandoned> ctx, UpdateDefinitionBuilder<ReadModelDocument> update)
    {
        var evt = ctx.Message;
        return update.SetOnInsert(x => x.Id, ctx.Stream.GetId())
                 .Set(x => x.EmployeeId, evt.EmployeeID)
                 .Set(x => x.ApprovedAt, evt.ApprovedAt)
                 .Set(x => x.Reason, evt.Reason ?? string.Empty);
    }
}
