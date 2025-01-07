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
        //var connectionString = configuration
        //                    .GetConnectionString("ctbx-events-db")
        //                    !.GuardAgainstNullOrEmpty("ctbx-events-db");

        services.AddScoped<HolidaysImporter>();
        services.AddHostedService<ImportHolidaysDbSeeder>();
        //services.AddScoped<IFileUploadHandler, FileUploadService>();
        services.Configure<FileUploadOptions>(configuration.GetSection(nameof(FileUploadOptions)));
        //services.Configure<HolidayImporterOptions>(configuration.GetSection("POSTGRES_DB"));
        //services.Configure<HolidayImporterOptions>(configuration
        //                    .GetConnectionString("ctbx-events-db"));
        services.Configure<HolidayImporterOptions>(options =>
        { options.ConnectionString = configuration
                            .GetConnectionString("ctbx-events-db")
                            !.GuardAgainstNullOrEmpty("ctbx-events-db"); });
        services.AddScoped<FluentValidator>();
        services.AddScoped<FileImportService>();
        services.AddScoped<FileImporter>();

        

    }
}

