using System.Net.Http.Json;
using CTBX.AbsenceManagement.Shared.DTOs;

namespace CTBX.AbsenceManagement.UI
{
    public class AbsenceManagementService
    {

        private readonly HttpClient _httpClient;
        public AbsenceManagementService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }
        public async Task<HttpResponseMessage> SendCommand(VacationScheduleCommand command)
        {
            return await _httpClient.PostAsJsonAsync(BackendRoutes.VacationScheduleURL, command);
        }

        public async Task<HttpResponseMessage> EditCommand(VacationChangeCommand command)
        {
            return await _httpClient.PostAsJsonAsync(BackendRoutes.VacationScheduleEditURL, command);   
        }

        public async Task<List<VacationScheduleDTO>> GetVacationSchedulesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<VacationScheduleDTO>>(BackendRoutes.VacationDatagridURL) ?? new List<VacationScheduleDTO>();
        }

        public async Task<List<DraftsItems>> GetVacationSchedulesCalenderAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<DraftsItems>>(BackendRoutes.VacationCalenderViewURL) ?? new List<DraftsItems>();
        }
    }
}
