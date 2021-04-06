using Trac.Core.DM.Manager.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Trac.Core.DM.Manager.Interfaces
{
    public interface IDPSRegistryManager
    {
        public Task<DPSDeviceRegOperationResult> RegisterDeviceAsync(IotDevice iotHubDevice, IDeviceRegistryManager deviceRegistryMgr, IConfiguration configuration);

        

    }
}
