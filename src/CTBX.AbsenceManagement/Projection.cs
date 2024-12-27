using Eventuous.Postgresql.Projections;
using Eventuous;
using Npgsql;
using Microsoft.Extensions.Logging;

public class Projection : PostgresProjector
{
    readonly ILogger<Projection> _logger;
    public Projection(NpgsqlDataSource dataSource) : base(dataSource)
    {
           
    }
}

