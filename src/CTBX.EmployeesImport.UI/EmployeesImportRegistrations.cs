using Microsoft.Extensions.DependencyInjection;

namespace CTBX.EmployeesImport.UI;

public static class EmployeesImportRegistrations
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, string baseAddress)
    {
        services.AddHttpClient<UploadEmployeesService>(opts => opts.BaseAddress = new Uri(baseAddress));
        return services;
    }
}
