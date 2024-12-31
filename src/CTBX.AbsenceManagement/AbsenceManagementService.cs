using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventuous;
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
        public async Task<List<VacationScheduleCommand>> GetAllVacationSchedules()
        {
                var vacationSchedules = await _vacationSchedules.Find(_ => true).ToListAsync();
            //calls out all documents from vacation db in moggo
                return vacationSchedules;
        }


    }
}
