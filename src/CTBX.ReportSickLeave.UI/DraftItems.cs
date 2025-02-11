using System.Drawing;
using Heron.MudCalendar;

namespace CTBX.ReportSickLeave.UI;

public class DraftItems : CalendarItem
{
    public string Title { get; set; } = string.Empty;
    public Color Color { get; set; }
}
