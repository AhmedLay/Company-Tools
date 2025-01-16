using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CTBX.SkillManagment.Backend;

public static class SkillManagmentFeatureRegistration
{
    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<SkillsManager>();
        services.Configure<SkillManagerOptions>(options=> options.ConnectionString = configuration.GetConnectionString("ctbx-common-db").GuardAgainstNull("ctbx-common-db")!);
        services.AddHostedService<SkillsManagmentDataSeeder>();
    }
}

