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
    }
}
