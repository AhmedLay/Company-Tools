using System.Reflection;
using CTBX.AbsenceManagement.UI;
using CTBX.EmployeesImport.UI;
using CTBX.ImportHoliday.UI;

namespace CTBX.WebPortal.Client;

public static class Features
{
    public static Assembly[] Assemblies
    => [
        typeof(Program).Assembly,
        typeof(EmployeesImportRegistrations).Assembly,
        typeof(AbsenceManagementRegistration).Assembly,
        typeof(ImportHolidaysRegistration).Assembly,
       ];
}
