using System.Drawing;
using CTBX.AbsenceManagement.Shared.AbsenceManagerCommands;
using CTBX.AbsenceManagement.Shared.DTOs;
using MongoDB.Driver;

namespace MinimalApiArchitecture.Application
{
    public class AbsenceManagementService
    {
        private readonly IMongoCollection<VacationScheduled> _vacationSchedules;
        //private readonly IMongoCollection<re> _sickLeaveSchedules;

        public AbsenceManagementService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ctbx-read-db"); 
            _vacationSchedules = database.GetCollection<VacationScheduled>("Readmodel");
            //_sickLeaveSchedules = database.GetCollection<RequestSickLeave>("Readmodel");
        }

        public async Task<List<VacationScheduleDTO>> GetDataTest()
        {
            var vacationSchedules = new List<VacationScheduleDTO>
        {
        new VacationScheduleDTO
            {
            Id = "1",
            From = DateTimeOffset.UtcNow.AddDays(1),
            To = DateTimeOffset.UtcNow.AddDays(7),
            Comment = "Urlaub für Familie",
             }};
            return await Task.FromResult(vacationSchedules);
        }
        public async Task<List<VacationScheduleDTO>> GetData()
        {
            var projection = Builders<VacationScheduled>.Projection
                .Include(e => e.EmployeeID)
                .Include(e => e.From)
                .Include(e => e.To)
                .Include(e => e.Comment);

            var vacationScheduleCommands = await _vacationSchedules
                .Find(FilterDefinition<VacationScheduled>.Empty)
                .Project<VacationScheduled>(projection)
                .ToListAsync();

            var listofdrafts = vacationScheduleCommands.Select(command => new VacationScheduleDTO
            {
                Id = command.EmployeeID.ToString(),
                From = command.From,
                To = command.To,
                Comment = command.Comment,
            }).ToList();

            return listofdrafts;
        }

       

        public async Task<List<DraftsItems>> GetCalenderData()
        {
            var projection = Builders<VacationScheduled>.Projection
                .Include(e => e.EmployeeID)
                .Include(e => e.From)
                .Include(e => e.To)
                .Include(e => e.Comment);

            var vacationScheduleCommands = await _vacationSchedules
                .Find(FilterDefinition<VacationScheduled>.Empty)
                .Project<VacationScheduled>(projection)
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
