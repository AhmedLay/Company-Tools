using Heron.MudCalendar;

    public class ReadModel : CalendarItem
    {
        public required string id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public int EmployeeID { get; set; }
        public string Status { get; set; } = string.Empty;
        public MudBlazor.Color Color { get; set; } = MudBlazor.Color.Default;
}

