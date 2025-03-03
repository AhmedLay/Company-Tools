using CTBX.AbsenceManagement.Shared;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Npgsql;
using Dapper;
using MudBlazor;


namespace MinimalApiArchitecture.Application
{
    public class AbsenceManagementService
    {
        private readonly IMongoCollection<ReadModelDocument> _vacationSchedules;
        private readonly string? _connectionString;

        public AbsenceManagementService(IMongoClient mongoClient, IConfiguration configuration)
        {
            var database = mongoClient.GetDatabase("ctbx-read-db");
            _vacationSchedules = database.GetCollection<ReadModelDocument>("ReadModel");
            _connectionString = configuration.GetConnectionString("ctbx-common-db")!;
        }
        public async Task<List<ReadModel>> GetData()
        {
            var filter = Builders<ReadModelDocument>.Filter.In(e => e.Status, new[] { "Drafted", "Requested","Approved", "Rejected" });
            var projection = Builders<ReadModelDocument>.Projection
                .Include(e => e.EmployeeId)
                .Include(e => e.Id)
                .Include(e => e.From)
                .Include(e => e.To)
                .Include(e => e.Comment)
                .Include(e => e.Status);

            var vacationScheduleCommands = await _vacationSchedules
                .Find(filter)
                .Project<ReadModelDocument>(projection)
                .ToListAsync();
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var listOfDrafts = new List<ReadModel>();

            foreach (var command in vacationScheduleCommands)
            {
                var lastName = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT name FROM public.employees WHERE employeeid = @ID",
                    new { ID = command.EmployeeId }
                );

                listOfDrafts.Add(new ReadModel
                {
                    EmployeeID = command.EmployeeId,
                    LastName = lastName ?? "Unknown",
                    id = command.Id,
                    Start = command.From.DateTime,
                    End = command.To.DateTime,
                    Text = command.Comment,
                    Status = command.Status
                });
            }

            return listOfDrafts;
        }
        public async Task<int> GetIdfromEmployees(string email)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            var result = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT employeeid FROM public.employees WHERE email = @Email",
                new { Email = email }
            );
            return result;
        }
}
}
