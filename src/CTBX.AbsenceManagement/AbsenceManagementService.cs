using System.Drawing;
using CTBX.AbsenceManagement.Shared.AbsenceManagerCommands;
using CTBX.AbsenceManagement.Shared.DTOs;
using MinimalApiArchitecture.Application.Commands;
using MongoDB.Driver;

namespace MinimalApiArchitecture.Application
{
    public class AbsenceManagementService
    {
        private readonly IMongoCollection<VacationScheduleCommand> _vacationSchedules;
        private readonly IMongoCollection<RequestSickLeave> _sickLeaveSchedules;

        public AbsenceManagementService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ctbx-read-db"); 
            _vacationSchedules = database.GetCollection<VacationScheduleCommand>("Vacation");
            _sickLeaveSchedules = database.GetCollection<RequestSickLeave>("Vacation");
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
            var projection = Builders<VacationScheduleCommand>.Projection
                .Include(e => e.Id)
                .Include(e => e.From)
                .Include(e => e.To)
                .Include(e => e.Comment);

            var vacationScheduleCommands = await _vacationSchedules
                .Find(FilterDefinition<VacationScheduleCommand>.Empty)
                .Project<VacationScheduleCommand>(projection)
                .ToListAsync();

            var listofdrafts = vacationScheduleCommands.Select(command => new VacationScheduleDTO
            {
                Id = command.Id,
                From = command.From,
                To = command.To,
                Comment = command.Comment,
            }).ToList();

            return listofdrafts;
        }

        public async Task<List<SickLeaveDTO>> GetSickLeaveData()
        {
            try
            {
                var projection = Builders<RequestSickLeave>.Projection
                .Include(e => e.Id)
                .Include(e => e.From)
                .Include(e => e.Until)
                .Include(e => e.Comment);

                var sickLeaveScheduleCommands = await _sickLeaveSchedules
                    .Find(FilterDefinition<RequestSickLeave>.Empty)
                    .Project<RequestSickLeave>(projection)
                    .ToListAsync();

                var listofsickleaves = sickLeaveScheduleCommands.Select(command => new SickLeaveDTO
                {
                    Id = command.Id,
                    From = command.From,
                    Until = command.Until,
                    Comment = command.Comment ?? string.Empty,
                    ReportedAt = command.ReportedAt,
                }).ToList();

                return listofsickleaves;
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using ILogger)
                throw new ApplicationException("An error occurred while retrieving sick leave data.", ex);
            }
        }

        public async Task<List<DraftsItems>> GetCalenderData()
        {
            var projection = Builders<VacationScheduleCommand>.Projection
                .Include(e => e.Id)
                .Include(e => e.From)
                .Include(e => e.To)
                .Include(e => e.Comment);

            var vacationScheduleCommands = await _vacationSchedules
                .Find(FilterDefinition<VacationScheduleCommand>.Empty)
                .Project<VacationScheduleCommand>(projection)
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
