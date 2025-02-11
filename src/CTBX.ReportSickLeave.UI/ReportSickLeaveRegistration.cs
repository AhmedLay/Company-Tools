
using Microsoft.Extensions.DependencyInjection;

namespace CTBX.ReportSickLeave.UI;

public static class ReportSickLeaveRegistration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, string baseAdress)
    {
        services.AddHttpClient<ReportSickLeaveService>(OptionsServiceCollectionExtensions => OptionsServiceCollectionExtensions.BaseAddress = new Uri(baseAdress));
        return services;

    }
}
