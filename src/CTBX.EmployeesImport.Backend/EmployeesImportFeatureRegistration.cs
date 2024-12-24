using CTBX.EmployeesImport.Backend;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace MinimalApiArchitecture.Application;

public static class EmployeesImportFeatureRegistration
{
    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<EmployeeRegistrationDbSeeder>();
        services.AddScoped<IFileUploadHandler, FileUploadService>();
        services.Configure<FileUploadOptions>(configuration.GetSection(nameof(FileUploadOptions)));
        services.AddScoped<FluentValidator>();
        services.AddScoped<FileImportService>();
        services.AddScoped<FileImporter>();
    }
}
