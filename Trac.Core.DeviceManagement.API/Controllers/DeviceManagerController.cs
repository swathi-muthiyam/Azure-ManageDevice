// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceManagerController.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using Trac.Core.ADT.Manager.Interfaces;
using Trac.Core.DM.Manager.Interfaces;
using Trac.Core.DM.Manager.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using static Trac.Core.DM.Manager.Utils.DeviceManagerUtils;
using IotDevice = Trac.Core.DM.Manager.Models.IotDevice;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Trac.Core.DeviceManagement.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class DeviceManagerController : ControllerBase
    {
        private readonly IDeviceRegistryManager _deviceRegistryManager;

        /// <summary>
        /// Device Manager Controller
        /// </summary>
        /// <param name="deviceRegistryManager"></param>
        /// <param name="deviceService"></param>
        public DeviceManagerController(IDeviceRegistryManager deviceRegistryManager, IDeviceService deviceService)
        {
            this._deviceRegistryManager = deviceRegistryManager;
        }

        /// <summary>
        /// Register Device
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("iotdevicess")]
        public async Task<ContentResult> RegisterDevice(IotDevice deviceInfo, long userId)
        {
            var result = await this._deviceRegistryManager.RegisterDevice(deviceInfo);


            return this.Content(result.IsSuccessful ? "Registered." : result.Message);
        }

        /// <summary>
        /// De-Register Device
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("DeRegisterDevice")]
        public async Task<ContentResult> DeRegisterDevice(IotDevice deviceInfo, long userId)
        {
            var result = await this._deviceRegistryManager.RemoveDeviceAsync(deviceInfo);
            return this.Content(result.IsSuccessful ? "DeRegistered." : "Not able to deregister!");
        }

        /// <summary>
        /// Update Device
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut("UpdateDevice")]
        public async Task<ContentResult> UpdateRegisteredDevice(IotDevice deviceInfo, long userId)
        {
            var result = await this._deviceRegistryManager.UpdateDeviceAsync(deviceInfo);
            return this.Content(result.IsSuccessful ? "Registered." : "Not able to register!");
        }

        /// <summary>
        /// Get devices 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Listiotdevices")]
        public async Task<DeviceRegistryOperationResult> GetListOfRegisteredDevices()
        {

            await this._deviceRegistryManager.GetRegistredConfigurationsAsync();


            var result = await this._deviceRegistryManager.GetDevicesAsync();
            if (result == null)
            {
                return new DeviceRegistryOperationResult() { Status = (Microsoft.Azure.OperationStatus)OperationStatus.Failure };
            }
            return new DeviceRegistryOperationResult() { IotDevicesData = result, Status = (Microsoft.Azure.OperationStatus)OperationStatus.Success };
        }

        /// <summary>
        /// Get devices 
        /// </summary>
        /// <returns></returns>
        [HttpPost("iotdevices")]
        public async Task<DeviceRegistryOperationResult> GetListOfRegisteredDevices(IotDevice hub)
        {

           // await this._deviceRegistryManager.GetRegistredConfigurationsAsync();


            var result = await this._deviceRegistryManager.GetDevicesAsync(hub);
            if (result == null)
            {
                return new DeviceRegistryOperationResult() { Status = (Microsoft.Azure.OperationStatus)OperationStatus.Failure };
            }
            return new DeviceRegistryOperationResult() { IotDevicesData = result, Status = (Microsoft.Azure.OperationStatus)OperationStatus.Success };
        }

        /// <summary>
        /// Upload Bulk Devices file
        /// </summary>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<DeviceRegistryOperationResult> UploadFileAsync(IFormFile deviceRegistrationFile)
        {
            HttpResponseMessage result = null;
            string blobName = deviceRegistrationFile.FileName;
            var fileContent = new StringBuilder();

            if (deviceRegistrationFile.Length == 0)
            {
                return await Task.FromResult(new DeviceRegistryOperationResult { IsSuccessful = false, Message = "Uploaded file doesn't contain valid data" });
            }

            using (var reader = new StreamReader(deviceRegistrationFile.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    fileContent.AppendLine(await reader.ReadLineAsync());
            }

            var fileContentData = Encoding.ASCII.GetBytes(fileContent.ToString());

            return await this._deviceRegistryManager.UploadBulkDevicesToStorage(blobName, fileContentData);
        }
        /// <summary>
        /// Download Bulk Devices file
        /// </summary>
        /// <returns></returns>
        [HttpPost("download")]
        public async Task<DeviceRegistryOperationResult> DownloadFileAsync(StorageProperties properties)
        {
            return await this._deviceRegistryManager.DownloadBlob(properties.BlobName, properties.ContainerName, @"D:\");

        }

        /// <summary>
        /// Register Bulk Devices
        /// </summary>
        /// <returns></returns>
        [HttpPost("bulkupload")]
        public async Task<IEnumerable<DeviceRegistryOperationResult>> RegisterBulkDevicesAsync([FromBody] object request)
        {
            return await this._deviceRegistryManager.RegisterBulkDevices(request);

        }

        /// <summary>
        /// Create Device Twin Configuration
        /// </summary>
        /// <returns></returns>
        public async Task<DeviceRegistryOperationResult> CreateDeviceTwinConfiguration()
        {

            //var cts = new CancellationTokenSource(TimeSpan.FromSeconds(int.Parse(_registerConfig["DeviceManagement:Registration:registrationTimeOutInSeconds"])));
            //var result = await this._deviceRegistryManager.(_registerConfig["DeviceManagement:Registration:IotHubConnectionString"], cts.Token);

            return new DeviceRegistryOperationResult() { Status = (Microsoft.Azure.OperationStatus)OperationStatus.Success }; ;
        }


    }
}
