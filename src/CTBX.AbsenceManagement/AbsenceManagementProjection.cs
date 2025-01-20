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

public class AbsenceManagementProjection : MongoProjector<VacationDocument>
{
    private readonly ILogger<AbsenceManagementProjection> _logger;
    public AbsenceManagementProjection(IMongoDatabase client, ILogger<AbsenceManagementProjection> logger) : base(client)
    {
        _logger = logger;
        On<VacationScheduled>(aggregate => aggregate.GetId(), Handle);
        On<VacationChanged>(aggregate => aggregate.GetId(), Handle);
    }
    static UpdateDefinition<VacationDocument> Handle(
        IMessageConsumeContext<VacationScheduled> ctx, UpdateDefinitionBuilder<VacationDocument> update)
    {
        var evt = ctx.Message;

        return update.SetOnInsert(x => x.Id, ctx.Stream.GetId())          
                 .Set(x => x.EmployeeId, evt.EmployeeID)                   
                 .Set(x => x.From, evt.From)                            
                 .Set(x => x.To, evt.To)                              
                 .Set(x => x.ScheduledAt, evt.ScheduledAt)               
                 .Set(x => x.Comment, evt.Comment ?? string.Empty);

    }

    static UpdateDefinition<VacationDocument> Handle(
        IMessageConsumeContext<VacationChanged> ctx, UpdateDefinitionBuilder<VacationDocument> update)
    {
        var evt = ctx.Message;
        return update.Set(x => x.Id, ctx.Stream.GetId())
                 .Set(x => x.EmployeeId, evt.EmployeeID)
                 .Set(x => x.From, evt.From)
                 .Set(x => x.To, evt.To)
                 .Set(x => x.Comment, evt.Comment ?? string.Empty);
    }
}



