using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CTBX.ReportSickLeave.UI
{
    public static class ReportSickLeaveRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, string baseAdress)
        {
            services.AddHttpClient<ReportSickLeave>(OptionsServiceCollectionExtensions => OptionsServiceCollectionExtensions.BaseAddress = new Uri(baseAdress));
            return services;

        }
    }
}
