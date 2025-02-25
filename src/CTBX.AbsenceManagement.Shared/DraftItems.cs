using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heron.MudCalendar;

    public class DraftsItems : CalendarItem
    {
        public required string id { get; set; }
        public string Status { get; set; } = string.Empty;
        public Color Color { get; set; } 
    }

