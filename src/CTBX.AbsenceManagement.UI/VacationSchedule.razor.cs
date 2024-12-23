using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace CTBX.AbsenceManagement.UI
{
    public class VacationScheduleFileBase : ComponentBase
    {
        public record Request(int Id, string Draftname, DateTimeOffset From, DateTimeOffset To, string AbsenceType);
        public List<Request> Requests { get; set; } = new();
        public bool _open = false;
        public string Draftname { get; set; } = string.Empty;
        public DateTimeOffset From { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset To { get; set; } = DateTimeOffset.Now;
        public string Comment { get; set; } = string.Empty;
        public bool Dense_Radio { get; set; } = true;

        public void OpenDrawer()
        {
            _open = true;
        }

        public void SaveDraft()
        {
            var newRequest = new Request(
                Id: Requests.Count + 1,
                Draftname: Draftname,
                From: From,
                To: To,
                AbsenceType: Dense_Radio ? "Vacation" : "Sick Leave"
            );

            Requests.Add(newRequest);

            // Reset form fields
            Draftname = string.Empty;
            From = DateTimeOffset.Now;
            To = DateTimeOffset.Now;
            Comment = string.Empty;
            Dense_Radio = true;

            // Close drawer
            _open = false;

            Console.WriteLine($"Draft '{newRequest.Draftname}' saved successfully!");
        }

        public void SubmitRequest()
        {
            Console.WriteLine("Vacation request submitted!");
        }
    }
}
