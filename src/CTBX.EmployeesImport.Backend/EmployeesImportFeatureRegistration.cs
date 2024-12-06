using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MinimalApiArchitecture.Application;

public static class EmployeesImportFeatureRegistration
{
    public static void RegisterServices(IServiceCollection services,IConfiguration configuration)
    {
        services.AddSingleton<DataContext>();
        services.AddHostedService<DataContextInitializer>();
    }

    public class DataContextInitializer : IHostedService
    {
        private readonly DataContext _dataContext;

        public DataContextInitializer(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting DataContextInitializer...");
            await _dataContext.Init();
            Console.WriteLine("DataContextInitializer completed.");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }


}

