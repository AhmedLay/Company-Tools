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
        services.AddScoped<HolidaysImporter>();
        services.AddHostedService<ImportHolidaysDbSeeder>();
        services.AddScoped<IFileUploadHandler, FileUploadService>();
        services.Configure<FileUploadOptions>(configuration.GetSection(nameof(FileUploadOptions)));
        services.Configure<FileUploadOptions>(configuration.GetSection(nameof(HolidayImporterOptions)));
        services.AddScoped<FluentValidator>();
        services.AddScoped<FileImportService>();
        services.AddScoped<FileImporter>();


    }
}

