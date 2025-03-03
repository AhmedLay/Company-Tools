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
        public async Task<HttpResponseMessage> ApproveVacation(ApprovingVacation command)
        {
            return await _httpClient.PostAsJsonAsync(BackendRoutes.ApproveVacationURL, command);
        }
        public async Task<HttpResponseMessage> RejectRequest(RejectingRequest command)
        {
            return await _httpClient.PostAsJsonAsync(BackendRoutes.RejectRequestURL, command);
        }
        public async Task<List<ReadModel>> GetData()
        {
            return await _httpClient.GetFromJsonAsync<List<ReadModel>>(BackendRoutes.VacationCalenderViewURL) ?? new List<ReadModel>();
        }
        public async Task<HttpResponseMessage> RequestVacation(RequestingVacation command)
        {
            return await _httpClient.PostAsJsonAsync(BackendRoutes.RequestVacationURL, command);
        }
        public async Task<HttpResponseMessage> AbondonRequest(AbdoningRequest command)
        {
            return await _httpClient.PostAsJsonAsync(BackendRoutes.AbondonRequestURL, command);
        }
        public async Task<int> GetEmployeeID(string email)
        {
            return await _httpClient.GetFromJsonAsync<int>($"{BackendRoutes.GetEmployeeID}?email={email}");
        }
    }
}
