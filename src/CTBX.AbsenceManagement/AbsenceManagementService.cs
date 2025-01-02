using CTBX.AbsenceManagement.Shared.DTOs;
using MinimalApiArchitecture.Application.Commands;
using MongoDB.Driver;

namespace MinimalApiArchitecture.Application
{
    public class AbsenceManagementService
    {
        private readonly IMongoCollection<VacationScheduleCommand> _vacationSchedules;

        public AbsenceManagementService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ctbx-read-db"); 
            _vacationSchedules = database.GetCollection<VacationScheduleCommand>("Vacation");
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

    }
}
