using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiArchitecture.Application;

public static class EmployeesImportFeatureRegistration
{
    public static void RegisterServices(IServiceCollection services,IConfiguration configuration)
    {
        services.AddSingleton<DataContext>();
    }

}

