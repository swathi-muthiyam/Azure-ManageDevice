using Trac.Core.ADT.Manager.Interfaces;
using Trac.Core.DigitalTwin.Manager;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;


[assembly: FunctionsStartup(typeof(Trac.DeviceEvents.FuncApp.Startup))]
namespace Trac.DeviceEvents.FuncApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {   
            
            builder.Services.AddSingleton<IADTClientManager>(s =>            
                 new ADTClientManager()
            );

            //builder.Services.AddSingleton<ILoggerProvider, Logger>();
        }
    }
}
