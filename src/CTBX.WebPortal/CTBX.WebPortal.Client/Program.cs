using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;




var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices();


builder.Services.AddScoped(sp =>
new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
//how to configure it right to backend 

await builder.Build().RunAsync();

