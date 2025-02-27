using CTBX.AbsenceManagement.Shared;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Npgsql;
using Dapper;
using Eventuous.Subscriptions.Context;
using Eventuous;


namespace MinimalApiArchitecture.Application
{
    public class AbsenceManagementService
    {
        private readonly IMongoCollection<ViewModel> _vacationSchedules;
        private readonly string? _connectionString;

        public AbsenceManagementService(IMongoClient mongoClient, IConfiguration configuration)
        {
            var database = mongoClient.GetDatabase("ctbx-read-db");
            _vacationSchedules = database.GetCollection<ViewModel>("ReadModel");
            _connectionString = configuration.GetConnectionString("ctbx-common-db")!;
        }
        public async Task<List<DraftsItems>> GetDataEmployee()
        {
            var filter = Builders<ViewModel>.Filter.In(e => e.Status, new[] { "Drafted", "Requested","Approved", "Rejected" });
            var projection = Builders<ViewModel>.Projection
                .Include(e => e.EmployeeId)
                .Include(e => e.Id)
                .Include(e => e.From)
                .Include(e => e.To)
                .Include(e => e.Comment)
                .Include(e =>e.Status);

            var vacationScheduleCommands = await _vacationSchedules
                .Find(filter)
                .Project<ViewModel>(projection)
                .ToListAsync();

            var listofdrafts = vacationScheduleCommands.Select(command => new DraftsItems
            {
                EmployeeID = command.EmployeeId,
                id = command.Id,
                Start = command.From.DateTime,
                End = command.To.DateTime,
                Text = command.Comment,
                Status = command.Status,

            }).ToList();

            return listofdrafts;
        }
        public async Task<List<DraftsItems>> GetDataSuperVisor()
        {
            var filter = Builders<ViewModel>.Filter.In(e => e.Status, new[] { "Requested" });
            var projection = Builders<ViewModel>.Projection
                .Include(e => e.EmployeeId)
                .Include(e => e.Id)
                .Include(e => e.From)
                .Include(e => e.To)
                .Include(e => e.Comment)
                .Include(e => e.Status);

            var vacationScheduleCommands = await _vacationSchedules
                .Find(filter)
                .Project<ViewModel>(projection)
                .ToListAsync();

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var listOfDrafts = new List<DraftsItems>();

            foreach (var command in vacationScheduleCommands)
            {
                var lastName = await connection.QueryFirstOrDefaultAsync<string>(
                    "SELECT name FROM public.employees WHERE employeeid = @ID",
                    new { ID = command.EmployeeId }
                );

                listOfDrafts.Add(new DraftsItems
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
