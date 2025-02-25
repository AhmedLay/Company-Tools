using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

     public class RequestModel
    {
        public string Id { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public DateTimeOffset Scheduledat { get; set; }
        public string Comment { get; set; } = string.Empty;
    }


//public enum RequestType
//{
//    Holidays,
//    SickLeave
//}

