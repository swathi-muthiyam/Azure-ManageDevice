// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeviceRegistryManager.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using Trac.Core.DM.Manager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trac.Core.DM.Manager.Interfaces
{
    public interface IDeviceRegistryManager
    {
        /// <summary>
        /// Register Device asynchronous
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="iotHubDevice"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DeviceRegistryOperationResult> RegisterDevice(IotDevice iotHubDevice);

        /// <summary>
        /// Registers the devices asynchronous.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="iotHubDevices">The iot hub devices.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The Device Registration Response</returns>
        //Task<IEnumerable<DeviceRegistryOperationResult>> RegisterDevicesAsync(string connectionString,string tag, IEnumerable<IotDevice> iotHubDevices, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the device asynchronous.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>success indicator</returns>
        Task<DeviceRegistryOperationResult> RemoveDeviceAsync(IotDevice iotDevice);

        /// <summary>
        /// Update Device asynchronous.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="iotHubDevice"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DeviceRegistryOperationResult> UpdateDeviceAsync(IotDevice iotHubDevice);

        /// <summary>
        /// Gets the device asynchronous.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Device if present, else null</returns>
        Task<IEnumerable<IotDevice>> GetDevicesAsync();
        Task<IEnumerable<IotDevice>> GetDevicesAsync(IotDevice iotDevice);

        public Task<IEnumerable<DeviceRegistryOperationResult>> RegisterBulkDevices(object request);

        public Task<DeviceRegistryOperationResult> UploadBulkDevicesToStorage(string BlobName, byte[] fileData);

        public Task<DeviceRegistryOperationResult> DownloadBlob(string blobToDownload, string containerName, string downloadsPath);

        public Task<DeviceRegistryOperationResult> GetRegistredConfigurationsAsync();
    }
}
