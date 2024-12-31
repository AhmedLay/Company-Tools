using System.Net.Http.Json;
using MinimalApiArchitecture.Application.Commands;

namespace CTBX.AbsenceManagement.UI
{
    public class AbsenceManagementService
    {

        private readonly HttpClient _httpClient;
        public AbsenceManagementService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }
        public async Task SendCommand(VacationScheduleCommand command)
        {
            var result = await _httpClient.PostAsJsonAsync(BackendRoutes.VacationScheduleURL, command);
        }

        public async Task<List<VacationScheduleCommand>> GetVacationSchedulesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<VacationScheduleCommand>>(BackendRoutes.VacationDatagridURL);
            return response ?? new List<VacationScheduleCommand>();
        }
    }
}
