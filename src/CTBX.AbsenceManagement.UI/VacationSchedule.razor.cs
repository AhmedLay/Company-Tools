using System;
using Microsoft.AspNetCore.Components;

namespace CTBX.AbsenceManagement.UI
{
    public class VacationScheduleFileBase : ComponentBase
    {
        public void SaveDraft()
        {
            Console.WriteLine("Draft saved successfully!");
        }

        public void SubmitRequest()
        {
            Console.WriteLine("Vacation request submitted!");
        }
    }
}
