using System.Drawing;
using CTBX.AbsenceManagement.Shared.AbsenceManagerCommands;
using CTBX.AbsenceManagement.Shared.DTOs;
using MongoDB.Driver;

namespace MinimalApiArchitecture.Application
{
    public class AbsenceManagementService
    {
        private readonly IMongoCollection<SchedulingVacation> _vacationSchedules;

        public AbsenceManagementService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ctbx-read-db");
            _vacationSchedules = database.GetCollection<SchedulingVacation>("ReadModel");
        }
        public async Task<List<DraftsItems>> GetCalenderData()
        {
            var projection = Builders<SchedulingVacation>.Projection
                .Exclude("_id")
                .Include(e => e.From)
                .Include(e => e.To)
                .Include(e => e.Comment);

            var vacationScheduleCommands = await _vacationSchedules
                .Find(FilterDefinition<SchedulingVacation>.Empty)
                .Project<SchedulingVacation>(projection)
                .ToListAsync();

            var listofdrafts = vacationScheduleCommands.Select(command => new DraftsItems
            {
                Start = command.From.DateTime,
                End = command.To.DateTime,
                Text = command.Comment,

            }).ToList();

            return listofdrafts;
        }

    }
}
