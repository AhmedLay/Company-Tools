
using Microsoft.Extensions.DependencyInjection;

namespace CTBX.ImportHoliday.UI
{
    public static class ImportHolidaysRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, string baseAddress)
        {
            services.AddHttpClient<ImportHolidaysService>(opts => opts.BaseAddress = new Uri(baseAddress));
            return services;
        }
    }
}
