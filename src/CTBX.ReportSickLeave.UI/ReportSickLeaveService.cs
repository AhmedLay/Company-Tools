
using System.Net.Http.Json;
using CTBX.ReportSickLeave.Shared;

namespace CTBX.ReportSickLeave.UI;

public class ReportSickLeaveService
{
    private readonly HttpClient _httpClient;
    public ReportSickLeaveService(HttpClient httpClient)
    {
        _httpClient = httpClient;

    }
    public async Task<HttpResponseMessage> SendCommand(ReportSickLeave command)
    {
        return await _httpClient.PostAsJsonAsync(BackendRoutes.SICKLEAVEREQUEST, command);

    }

    public async Task<List<SickLeaveReport>> GetVacationSchedulesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<SickLeaveReport>>(BackendRoutes.SICKLEAVEDATA) ?? new List<SickLeaveReport>();
    }
    public async Task<List<DraftItems>> GetVacationSchedulesCalenderAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<DraftItems>>(BackendRoutes.SICKLEAVEREQUEST) ?? new List<DraftItems>();
    }

}
