using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CTBX.CommonUtils;

namespace MinimalApiArchitecture.Application;

public static class CommonUtilRegistration
{
    public static void RegisterServices(IServiceCollection services,IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    }
}
