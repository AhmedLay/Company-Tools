
namespace CTBX.ImportHolidays.Shared;

public class Holiday
{
    public Holiday()
    {
    }
    public int Id { get; set; }
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string HolidayName { get; set; } = string.Empty;
    public DateTime HolidayDate { get; set; } 
    public bool IsGlobal { get; set; }
}
