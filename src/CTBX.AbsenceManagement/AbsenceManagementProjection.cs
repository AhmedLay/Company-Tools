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
using System.Runtime.Intrinsics;

public class AbsenceManagementProjection : MongoProjector<ReadModelDocument>
{
    public AbsenceManagementProjection(IMongoDatabase client) : base(client) {

        On<VacationScheduled>(aggregate => aggregate.GetId(), Handle);
        On<SickLeaveRequested>(aggregate => aggregate.GetId(), Handle);
    }
    static UpdateDefinition<ReadModelDocument> Handle(
        IMessageConsumeContext<VacationScheduled> ctx, UpdateDefinitionBuilder<ReadModelDocument> update)
    {
        var evt = ctx.Message;

        return update.SetOnInsert(x => x.Id, ctx.Stream.GetId())          
                 .Set(x => x.EmployeeId, evt.EmployeeID)                   
                 .Set(x => x.From, evt.From)                            
                 .Set(x => x.To, evt.To)                              
                 .Set(x => x.ScheduledAt, evt.ScheduledAt)               
                 .Set(x => x.Comment, evt.Comment ?? string.Empty);

    }

    static UpdateDefinition<ReadModelDocument> Handle(
        IMessageConsumeContext<SickLeaveRequested> ctx, UpdateDefinitionBuilder<ReadModelDocument> update)
    {
        var evt = ctx.Message;

        return update.SetOnInsert(x => x.Id, ctx.Stream.GetId())
                 .Set(x => x.EmployeeId, evt.EmployeeId)
                 .Set(x => x.From, evt.From)
                 .Set(x => x.To, evt.Until)
                 .Set(x => x.ScheduledAt, evt.ReportedAt);

    }

}



