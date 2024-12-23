using CTBX.CommonMudComponents;
using CTBX.EmployeesImport.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static MudBlazor.CategoryTypes;



namespace CTBX.EmployeesImport.UI
{
    public class EmployeeViewBase : BaseMudComponent
    {
        [Inject]
        public required UploadEmployeesService Service { get; set; }
        public List<Employee> EmployeeList = new();
        public string _searchString = string.Empty;
        public bool _sortNameByLength;

        protected override async Task OnInitializedAsync()
        {
            await ReloadData();
        }

        public Func<Employee, object> _sortBy => x =>
        {
            if (_sortNameByLength)
                return x.Name;
            else
                return x.EmployeeID;
        };

        public Func<Employee, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (x.Surname.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.EmployeeID.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.AnualVacationDays.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.RemainingVacationDays.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

        public async Task ReloadData()
        {
            EmployeeList = await Service.GetEmployeesAsync() ?? new List<Employee>();
        }

    }
}
