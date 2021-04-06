// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DPSManagerController.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Trac.Core.DM.Manager.Interfaces;
using Trac.Core.DM.Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Trac.Core.DeviceManagement.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class DPSManagerController : ControllerBase
    {
        private readonly IDPSRegistryManager _deviceRegistryManager;
        private readonly IDeviceRegistryManager _iotHubdeviceRegistryManager;
        private IConfiguration ConfigRoot;
        public DPSManagerController(IDPSRegistryManager deviceRegistryManager, IDeviceRegistryManager iotHubdeviceRegistryManager, IConfiguration configuration)
        {
            this._deviceRegistryManager = deviceRegistryManager;
            this._iotHubdeviceRegistryManager = iotHubdeviceRegistryManager;
            ConfigRoot = configuration;
        }

        [HttpPost("devices")]
        public Task<DPSDeviceRegOperationResult> RegisterDevice(IotDevice deviceInfo)
        {
            var result = _deviceRegistryManager.RegisterDeviceAsync(deviceInfo, _iotHubdeviceRegistryManager, ConfigRoot);
            return result;

            //return this.Content("Hub: " + result.DeviceRegResult.AssignedHub + "#Key:" + result.DeviceKey
            //    +"#DeviceId:" + result.DeviceRegResult.DeviceId + "#ErrorMessage:" + result.DeviceRegResult.ErrorMessage
            //    + "#ErrorCode:" + result.DeviceRegResult.ErrorCode);
        }
              

        [HttpGet("devices")]
        public async Task<string> GetRegisterDevice()
        {
            //var result = _deviceRegistryManager.RegisterDeviceAsync(deviceInfo);

            return "Successful!!"; //this.Content(result.IsSuccessful ? result.Message : $"Failed: {result.Message}");
        }

    }
}
