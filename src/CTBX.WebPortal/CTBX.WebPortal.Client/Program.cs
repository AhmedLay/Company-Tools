using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;



var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices();
builder.RegisterFeatures();
await builder.Build().RunAsync();
