//using CTBX.CommonMudComponents;
//using CTBX.EmployeesImport.Shared;
//using CTBX.ImportHoliday.UI;
//using CTBX.ImportHolidays.Shared;
//using Microsoft.AspNetCore.Components;
//using MudBlazor;
//using static MudBlazor.CategoryTypes;


//namespace CTBX.ImportHolidays.UI
//{
//    public class HolidayViewBase : BaseMudComponent
//    {
//        [Inject]
//        public required ImportHolidaysService Service { get; set; }
//        public List<Holiday> HolidaysList = new();
//        public string _searchString = string.Empty;
//        public bool _sortNameByLength;

//        protected override async Task OnInitializedAsync()
//        {
//            await ReloadData();
//        }

//        public Func<Holiday, object> _sortBy => x =>
//        {
//            if (_sortNameByLength)
//                return x.Country;
//            else
//                return x.HolidayDate;
//        };

//        public Func<Holiday, bool> _quickFilter => x =>
//        {
            
//            if (string.IsNullOrWhiteSpace(_searchString))
//                return true;

//            if (x.Country.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
//                return true;

//            if (x.HolidayDate.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
//                return true;

//            if (x.HolidayName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
//                return true;

//            if (x.IsGlobal.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
//                return true;

        
//            return false;
//        };

//        public async Task ReloadData()
//        {
            
//            var result = await service.getholidaysasync();
//            holidayslist = [.. result];
//        }

//    }
//}
