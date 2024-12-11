using CTBX.EmployeesImport.Backend;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiArchitecture.Application;

public static class EmployeesImportFeatureRegistration
{
    public static void RegisterServices(IServiceCollection services,IConfiguration configuration)
    {
        services.AddHostedService<EmployeeRegistrationDbSeeder>();
        services.AddScoped<IFileUploadHandler,FileUploadService>();
        services.Configure<FileUploadOptions>(configuration.GetSection(nameof(FileUploadOptions)));
    }
}
