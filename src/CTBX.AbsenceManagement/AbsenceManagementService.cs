using CTBX.AbsenceManagement.Shared;
using MongoDB.Driver;
using System.Drawing;

namespace MinimalApiArchitecture.Application
{
    public class AbsenceManagementService
    {
        private readonly IMongoCollection<ViewModel> _vacationSchedules;

        public AbsenceManagementService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ctbx-read-db");
            _vacationSchedules = database.GetCollection<ViewModel>("ReadModel");
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
    }
}
