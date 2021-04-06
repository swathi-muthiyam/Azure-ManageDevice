using Bosch.Trac360.Core.Common.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(Bosch.Trac360.BulkDeviceManagement.FuncApp.Startup))]
namespace Bosch.Trac360.BulkDeviceManagement.FuncApp
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient(); // Microsoft.Extensions.Http
            builder.Services.AddScoped<KeyVaultStoreManager>();

            // builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();

        }
    }
}
