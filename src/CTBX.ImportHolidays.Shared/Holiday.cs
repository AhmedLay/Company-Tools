
namespace CTBX.EmployeesImport.Shared;

public class Holiday
{
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string HolidayName { get; set; } = string.Empty;
    public DateTimeOffset HolidayDate { get; set; } 
    public bool IsGlobal { get; set; }
}
