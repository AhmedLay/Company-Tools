using CTBX.ImportHolidays.Backend;
using CTBX.ImportHolidays.UI;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CTBX.ImportHolidays.Backend;

public static class ImportHolidaysFeatureRegistration
{
    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ImportService>();
        services.AddHostedService<ImportHolidaysDbSeeder>();
        services.Configure<FileUploadOptions>(configuration.GetSection(nameof(FileUploadOptions)));
        services.Configure<HolidayImporterOptions>(options =>
        { options.ConnectionString = configuration
                            .GetConnectionString("ctbx-common-db")
                            !.GuardAgainstNullOrEmpty("ctbx-common-db"); });
        services.AddScoped<FluentValidator>();

    }
}

