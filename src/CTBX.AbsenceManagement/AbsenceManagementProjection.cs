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

public class AbsenceManagementProjection : MongoProjector<VacationDocument>
{
    public AbsenceManagementProjection(IMongoDatabase client) : base(client)
    {
        
    }

}



