using CTBX.EmployeesImport.UI;
using CTBX.WebPortal.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudBlazor.Translations;
using FluentValidation;





var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddValidatorsFromAssemblyContaining<FileUploadValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AbsenceManagementFormValidator>();

builder.Services.AddMudServices();
builder.Services.AddMudTranslations();
builder.RegisterFeatures();


await builder.Build().RunAsync();
