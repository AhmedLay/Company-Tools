using CTBX.AbsenceManagement.UI;
using Microsoft.Extensions.DependencyInjection;

namespace CTBX.EmployeesImport.UI;

public static class AbsenceManagementRegistration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, string baseAddress)
    {
        services.AddHttpClient<AbsenceManagementService>(opts => opts.BaseAddress = new Uri(baseAddress));
        return services;
    }
}
