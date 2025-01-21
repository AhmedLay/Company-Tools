using System.Collections.Immutable;
using CTBX.CommonMudComponents;
using CTBX.ImportHoliday.UI;
using CTBX.ImportHolidays.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static MudBlazor.CategoryTypes;


namespace CTBX.ImportHolidays.UI
{
    public class HolidayViewBase : BaseMudComponent
    {
        [Inject]
        public required ImportHolidaysService Service { get; set; }
        protected IImmutableList<Holiday> HolidaysList { get; set; } = ImmutableList<Holiday>.Empty;

        public string _searchString = string.Empty;
        public bool _sortNameByLength;

        protected override async Task OnInitializedAsync()
        {
            await ReloadHolidaysData();
        }

        public Func<Holiday, object> _sortBy => x =>
        {
            if (_sortNameByLength)
                return x.Country;
            else
                return x.HolidayDate;
        };

        public Func<Holiday, bool> _quickFilter => x =>
        {

            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.Country.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.HolidayDate.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.HolidayName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.IsGlobal.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;


            return false;
        };

        public async Task ReloadHolidaysData()
        {
            try
            {
                HolidaysList = await Service.GetHolidaysAsync();

            }
            catch (Exception ex)
            {
                await NotifyError($"Failed to reload Holidays: {ex.Message}");
                HolidaysList = ImmutableList<Holiday>.Empty; // Reset the list to an empty state
            }
        }
    }
}
