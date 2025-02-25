using System.Net.Http.Json;
using CTBX.AbsenceManagement.Shared;
using static CTBX.AbsenceManagement.UI.ScheduleFileBase;

namespace CTBX.AbsenceManagement.UI
{
    public class AbsenceManagementService
    {

        private readonly HttpClient _httpClient;
        public AbsenceManagementService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }
        public async Task<HttpResponseMessage> SetSchedule(SchedulingVacation command)
        {
            return await _httpClient.PostAsJsonAsync(BackendRoutes.VacationScheduleURL, command);
        }
        public async Task<HttpResponseMessage> SendCommandSL(RequestingSickLeave command)
        {
            return await _httpClient.PostAsJsonAsync(BackendRoutes.SICKLEAVEREQUEST, command);
        }
        public async Task<HttpResponseMessage> EditVacation(ChangingVacationSchedule command)
        {
            return await _httpClient.PostAsJsonAsync(BackendRoutes.EditVacation, command);
        }
        public async Task<List<DraftsItems>> GetVacationSchedulesCalenderAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<DraftsItems>>(BackendRoutes.VacationCalenderViewURL) ?? new List<DraftsItems>();
        }
        public async Task<HttpResponseMessage> RequestVacation(RequestingVacation command)
        {
            return await _httpClient.PostAsJsonAsync(BackendRoutes.RequestVacationURL, command);
        }
    }
}
