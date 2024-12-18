using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTBX.EmployeesImport.Backend
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string Surname { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int AnualVacationDays { get; set; }
        public int RemainingVacationDays { get; set; }
    }
}
