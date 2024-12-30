﻿using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CTBX.EmployeesImport.UI;
using CTBX.AbsenceManagement.UI;

public static class Registrations
{
    public static WebAssemblyHostBuilder RegisterFeatures(this WebAssemblyHostBuilder builder)
    {
        EmployeesImportRegistrations.RegisterServices(builder.Services, builder.HostEnvironment.BaseAddress);
        AbsenceManagementRegistration.RegisterServices(builder.Services, builder.HostEnvironment.BaseAddress);
        return builder;
    }
}
