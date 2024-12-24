using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class Employee
    {
        public int ID { get; set; }
        public int EmployeeID { get; set; }
        public string Surname { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int AnualVacationDays { get; set; }
        public int RemainingVacationDays { get; set; }
    }

