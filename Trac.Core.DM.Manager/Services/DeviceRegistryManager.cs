// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceRegistryManager.cs" company=" "> 
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
using Trac.Core.Common.Configuration;
using Trac.Core.DeviceManagement.API.Services;
using Trac.Core.DM.Manager.Interfaces;
using Trac.Core.DM.Manager.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trac.Core.DM.Manager.Services
{
    /// <summary>
    /// Device Registry Manager
    /// </summary>
    public class DeviceRegistryManager : IDeviceRegistryManager
    {
        private readonly ILogger<DeviceRegistryManager> _logger;
        private readonly IConfiguration _registerConfig;
        private readonly IDeviceService _deviceService;
        private readonly BlobOperation _blobOperation;

        Microsoft.Azure.Devices.Client.TransportType transport = Microsoft.Azure.Devices.Client.TransportType.Amqp;
        public DeviceRegistryManager(ILogger<DeviceRegistryManager> logger, IDeviceService deviceService)
        //TODO: Build logger  better way connecting storage , implement trabsient exception.
        ////ITransientFaultHandler<IotHubTransientErrorDetectionStrategy> transientFaultHandler)
        {
            _logger = logger;
            this._registerConfig = KeyVaultStoreManager.keyVaultStoreManagerInstance.GetValueFromKeyVault();
            this._deviceService = deviceService;
            this._blobOperation = new BlobOperation();
            //this.transientFaultHandler = transientFaultHandler;
        }

        /// <summary>
        /// Get Devices Asynchoronously
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IotDevice>> GetDevicesAsync()
        {
            var registryManager = RegistryManager.CreateFromConnectionString(_registerConfig[DMConstants.iothubconnectionstring]);
            List<IotDevice> iotdevices = new List<IotDevice>();
            var tag = "";
            try
            {
                // TODO: Transient retry parameters set from caller
                //var retryPolicy = this.transientFaultHandler.GetRetryPolicy();
                //var device = await retryPolicy.ExecuteAsync(() => registryManager.GetDeviceAsync(deviceId, cancellationToken), cancellationToken).ConfigureAwait(false);
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(int.Parse(_registerConfig["DeviceManagement:Registration:registrationTimeOutInSeconds"])));
                var devices = await registryManager.GetDevicesAsync(1000, cts.Token);
                tag = Convert.ToString(devices.Select(t => t.ETag).FirstOrDefault());
                if (devices != null)
                {

                    foreach (var device in devices)
                    {
                        var modules = await registryManager.GetModulesOnDeviceAsync(device.Id);
                        //iotdevices.Add(new IotDevice { Id = device.Id, Auth = device.Authentication.SymmetricKey.PrimaryKey });
                        iotdevices.Add(new IotDevice
                        {
                            Id = device.Id,
                            Auth = Convert.ToString(device.Authentication.Type),
                            ConnectionState = device.ConnectionState,
                            C2DMessageCount = device.CloudToDeviceMessageCount,
                            ConnectionStateUpdatedTime = device.ConnectionStateUpdatedTime,
                            ETag = device.ETag,
                            LastActivityTime = device.LastActivityTime,
                            Status = device.Status,
                            StatusReason = device.StatusReason,
                            StatusUpdatedTime = device.StatusUpdatedTime,
                            ModuleId = modules.Count() != 0 ? modules.FirstOrDefault().Id : String.Empty // TODO : Support multiple modules going forward.
                        });
                    }
                    return (IEnumerable<IotDevice>)iotdevices;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, string.Format(CultureInfo.InvariantCulture, "Device with tag: {0} could not be retrieved from IoT Hub", tag));
            }
            finally
            {
                await registryManager.CloseAsync().ConfigureAwait(false);
            }

            return null;
        }

        public async Task<IEnumerable<IotDevice>> GetDevicesAsync(IotDevice iotDevice)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(iotDevice.iothubConnection);
            List<IotDevice> iotdevices = new List<IotDevice>();
            var tag = "";
            try
            {
                // TODO: Transient retry parameters set from caller
                //var retryPolicy = this.transientFaultHandler.GetRetryPolicy();
                //var device = await retryPolicy.ExecuteAsync(() => registryManager.GetDeviceAsync(deviceId, cancellationToken), cancellationToken).ConfigureAwait(false);
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(200));
                //var cts = new CancellationTokenSource(TimeSpan.FromSeconds(int.Parse(_registerConfig["DeviceManagement:Registration:registrationTimeOutInSeconds"])));
                var devices = await registryManager.GetDevicesAsync(1000, cts.Token);
                tag = Convert.ToString(devices.Select(t => t.ETag).FirstOrDefault());
                if (devices != null)
                {

                    foreach (var device in devices)
                    {
                        var modules = await registryManager.GetModulesOnDeviceAsync(device.Id);
                        //iotdevices.Add(new IotDevice { Id = device.Id, Auth = device.Authentication.SymmetricKey.PrimaryKey });
                        iotdevices.Add(new IotDevice
                        {
                            Id = device.Id,
                            Auth = Convert.ToString(device.Authentication.Type),
                            ConnectionState = device.ConnectionState,
                            C2DMessageCount = device.CloudToDeviceMessageCount,
                            ConnectionStateUpdatedTime = device.ConnectionStateUpdatedTime,
                            ETag = device.ETag,
                            LastActivityTime = device.LastActivityTime,
                            Status = device.Status,
                            StatusReason = device.StatusReason,
                            StatusUpdatedTime = device.StatusUpdatedTime,
                            ModuleId = modules.Count() != 0 ? modules.FirstOrDefault().Id : String.Empty // TODO : Support multiple modules going forward.
                        });
                    }
                    return (IEnumerable<IotDevice>)iotdevices;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, string.Format(CultureInfo.InvariantCulture, "Device with tag: {0} could not be retrieved from IoT Hub", tag));
            }
            finally
            {
                await registryManager.CloseAsync().ConfigureAwait(false);
            }

            return null;
        }

        /// <summary>
        /// Register Device Asynchronously
        /// </summary>
        /// <param name="iotHubDevice"></param>
        /// <returns></returns>
        public async Task<DeviceRegistryOperationResult> RegisterDevice(IotDevice iotHubDevice)
        {
            //var auth = new AuthenticationMechanism { SymmetricKey = new SymmetricKey { PrimaryKey = iotHubDevice.Auth, SecondaryKey = iotHubDevice.Auth } };
            // Auto generating symetric key. if key is passed by end user then send Auth in new device object
            var newDevice = new Device(iotHubDevice.Id) { Status = DeviceStatus.Enabled, ETag = iotHubDevice.ETag }; //, Authentication = auth }; 
            var registryManager = RegistryManager.CreateFromConnectionString(_registerConfig[DMConstants.iothubconnectionstring]);
            Module module;
            try
            {
                // TODO: Transient retry parameters need to set from caller
                //var retryPolicy = this.transientFaultHandler.GetRetryPolicy();
                //var device = await retryPolicy.ExecuteAsync(() => registryManager.AddDeviceAsync(newDevice, cancellationToken), cancellationToken).ConfigureAwait(false);
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(int.Parse(_registerConfig[DMConstants.registrationTimeOutInSeconds])));
                var device = await registryManager.AddDeviceAsync(newDevice, cts.Token);
                if (device != null && !string.IsNullOrEmpty(device.GenerationId))
                {
                    _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, "Device with Id: {0} is registered in IoT Hub with Generation Id: {1}", iotHubDevice.Id, iotHubDevice.GenerationId));

                    var adtResult = await this._deviceService.RegisterDeviceInAdt(new ADT.Common.ViewModels.GroundSensorDeviceVM()
                    {
                        DeviceId = iotHubDevice.Id,
                        ETag = "Bosch Ground sensor", // TODO : Add etag to iothub property.
                        ParkingSlotId = iotHubDevice.SpaceId,
                        Status = (ADT.Common.Constants.DeviceStatus)DeviceStatus.Enabled
                    });
                    if (!adtResult)
                    {
                        _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, "Device with Id: {0} failed to register in IoT Hub with Generation Id: {1}", iotHubDevice.Id, iotHubDevice.GenerationId));
                        var resultRemoveDevice = await this.RemoveDeviceAsync(iotHubDevice);
                        {
                            if (!resultRemoveDevice.IsSuccessful)
                            {
                                _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, "Device {0} registration successful in IOT hub but, Registeration failed in digital twin. Please clean up the device from IOT hub, GenerationId : {1}", iotHubDevice.Id, iotHubDevice.GenerationId));
                                return new DeviceRegistryOperationResult() { Message = "Device " + iotHubDevice.Id + " registration successful in IOT hub but, Registeration failed in digital twin. Please clean up the device from IOT hub" };
                                // return this.Content();
                            }
                        }
                        return new DeviceRegistryOperationResult() { Message = "Registeration for device " + iotHubDevice.Id + "  failed in digital twin, Try registration again" };
                    }
                    else
                    {
                        try
                        {
                            iotHubDevice.ModuleId = !String.IsNullOrEmpty(iotHubDevice.ModuleId) ? iotHubDevice.ModuleId : "M" + iotHubDevice.Id;
                            module = await registryManager.AddModuleAsync(new Module(iotHubDevice.Id, iotHubDevice.ModuleId));
                            if (module != null && !string.IsNullOrEmpty(module.GenerationId))
                            {
                                return new DeviceRegistryOperationResult() { IsSuccessful = true, Message = string.Format(CultureInfo.InvariantCulture, "Device with Id: {0} & Module ID : {1} registered in IoT Hub & ADT", iotHubDevice.Id, iotHubDevice.ModuleId) };
                            }
                            else
                            {
                                // TODO : Logic to revert registration in IOT hub and ADT
                                return new DeviceRegistryOperationResult() { IsSuccessful = true, Message = string.Format(CultureInfo.InvariantCulture, "Module ID : {1} failed to register in device {0} in IoT Hub", iotHubDevice.Id, iotHubDevice.ModuleId) };
                            }

                        }
                        catch (ModuleAlreadyExistsException)
                        {
                            return new DeviceRegistryOperationResult() { IsSuccessful = false, Message = string.Format(CultureInfo.InvariantCulture, "Device with Id: {0} & Module ID : {1} already registered in IoT Hub", iotHubDevice.Id, iotHubDevice.ModuleId) };
                        }
                        catch (Exception ex)
                        {
                            // TODO : Logic to revert registration in IOT hub and ADT
                            return new DeviceRegistryOperationResult() { IsSuccessful = false, Message = string.Format(CultureInfo.InvariantCulture, "Device registeration failed in IoT Hub : {0} with exception {1}", iotHubDevice.Id, ex.Message) };
                        }

                    }

                }
                else
                {
                    return new DeviceRegistryOperationResult() { IsSuccessful = false, Message = string.Format(CultureInfo.InvariantCulture, "Device registeration failed in IoT Hub : {0}", iotHubDevice.Id) };
                }
            }
            catch (DeviceAlreadyExistsException dae)
            {

                var message = string.Format(CultureInfo.InvariantCulture, "Device with Id: {0} already exists in IoT Hub", iotHubDevice.Id);
                _logger.LogWarning(message);
                return new DeviceRegistryOperationResult() { IsSuccessful = false, Message = message };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, string.Format(CultureInfo.InvariantCulture, "Device with Id: {0} could not be registered in IoT Hub", iotHubDevice.Id));
                return new DeviceRegistryOperationResult() { IsSuccessful = false, Message = string.Format(CultureInfo.InvariantCulture, " {0} : " + ex.Message, iotHubDevice.Id) };
            }
            finally
            {
                await registryManager.CloseAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Remove Device Asynchronously
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public async Task<DeviceRegistryOperationResult> RemoveDeviceAsync(IotDevice deviceInfo)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(_registerConfig[DMConstants.iothubconnectionstring]);
            try
            {
                //var retryPolicy = this.transientFaultHandler.GetRetryPolicy();
                //await retryPolicy.ExecuteAsync(() => registryManager.RemoveDeviceAsync(deviceId, cancellationToken), cancellationToken).ConfigureAwait(false);

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(int.Parse(_registerConfig[DMConstants.registrationTimeOutInSeconds])));
                var newDevice = new Device(deviceInfo.Id) { ETag = deviceInfo.ETag }; //, Authentication = auth }; 
                await registryManager.RemoveDeviceAsync(deviceInfo.Id, cts.Token);

            }
            catch (DeviceNotFoundException dnf)
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Device with Id: {0} is not found in IoT Hub", deviceInfo.Id);
                _logger.LogError(dnf.Message);
                return new DeviceRegistryOperationResult() { IsSuccessful = false, Message = string.Format(CultureInfo.InvariantCulture, " {0} : " + dnf.Message, deviceInfo.Id) };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, string.Format(CultureInfo.InvariantCulture, "Device with Id: {0} could not be removed from IoT Hub", deviceInfo.Id));
                return new DeviceRegistryOperationResult() { IsSuccessful = false, Message = string.Format(CultureInfo.InvariantCulture, " {0} : " + ex.Message, deviceInfo.Id) };
            }
            finally
            {
                await registryManager.CloseAsync();
            }

            return new DeviceRegistryOperationResult() { IsSuccessful = true };
        }

        /// <summary>
        /// Update devices Asynchronously
        /// </summary>
        /// <param name="iotHubDevice"></param>
        /// <returns></returns>
        public async Task<DeviceRegistryOperationResult> UpdateDeviceAsync(IotDevice iotHubDevice)
        {
            var auth = new AuthenticationMechanism { SymmetricKey = new SymmetricKey { PrimaryKey = iotHubDevice.Auth, SecondaryKey = iotHubDevice.Auth } };
            var updatedDevice = new Device(iotHubDevice.Id) { Status = DeviceStatus.Disabled };//, Authentication = auth };
            var registryManager = RegistryManager.CreateFromConnectionString(_registerConfig[DMConstants.iothubconnectionstring]);
            try
            {
                // TODO: Transient retry parameters set from caller
                //var retryPolicy = this.transientFaultHandler.GetRetryPolicy();
                //var device = await retryPolicy.ExecuteAsync(() => registryManager.UpdateDeviceAsync(updatedDevice, true, cancellationToken), cancellationToken).ConfigureAwait(false);
                var device = await registryManager.UpdateDeviceAsync(updatedDevice, true);
                if (device != null && !string.IsNullOrEmpty(device.GenerationId))
                {
                    _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, "Device with Id: {0} is updated in IoT Hub with Generation Id: {1}", iotHubDevice.Id, device.Id));
                    return new DeviceRegistryOperationResult() { IsSuccessful = true };
                }
            }
            catch (DeviceNotFoundException dnf)
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Device with Id: {0} is not found in IoT Hub", iotHubDevice.Id);
                //_logger.LogError(dnf, message);
                return new DeviceRegistryOperationResult() { IsSuccessful = true, Message = message };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, string.Format(CultureInfo.InvariantCulture, "Device with Id: {0} could not be updated in IoT Hub", iotHubDevice.Id));
                return new DeviceRegistryOperationResult() { IsSuccessful = true, Message = ex.Message };
            }
            finally
            {
                await registryManager.CloseAsync();
            }

            return new DeviceRegistryOperationResult() { IsSuccessful = false };
        }

        /// <summary>
        /// Get Registred Configurations Asynchoronously
        /// </summary>
        /// <returns></returns>
        public async Task<DeviceRegistryOperationResult> GetRegistredConfigurationsAsync()
        {
            var registryManager = RegistryManager.CreateFromConnectionString(_registerConfig[DMConstants.iothubconnectionstring]);
            try
            {

                //var newDevice = new Device(deviceInfo.Id) { ETag = deviceInfo.ETag }; //, Authentication = auth }; 
                var configs = await registryManager.GetConfigurationsAsync(7);
                return new DeviceRegistryOperationResult() { IsSuccessful = false };

            }
            catch (DeviceNotFoundException dnf)
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Device with Id: {0} is not found in IoT Hub");
                //_logger.LogError(dnf, message);
                return new DeviceRegistryOperationResult() { IsSuccessful = false, Message = string.Format(CultureInfo.InvariantCulture, dnf.Message) };
            }

        }

        /// <summary>
        /// Bulk Register Devices Asynchronously
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="iotHubDevice"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<DeviceRegistryOperationResult> UploadBulkDevicesToStorage(string BlobName, byte[] fileData)
        {
            try
            {
                // set your container name
                var blobUploadStatus = await _blobOperation.UploadBlobAsync("bulkdevice", BlobName, Encoding.ASCII.GetString(fileData));

                if (blobUploadStatus)
                {
                    return new DeviceRegistryOperationResult { IsSuccessful = true, Message = "Successfully schedule bulk registration job, We will notify you further status via email." };
                }

                return new DeviceRegistryOperationResult { IsSuccessful = true, Message = "Failed uploading file, Contact your admin" };
            }
            catch (Exception ex)
            {
                return new DeviceRegistryOperationResult { IsSuccessful = false, Message = ex.Message };
            }

        }

        public async Task<DeviceRegistryOperationResult> DownloadBlob(string blobToDownload, string containerName, string downloadsPath)
        {
            try
            {
                // set your container name
                 _blobOperation.DownloadBlob(blobToDownload, containerName, downloadsPath);
                return new DeviceRegistryOperationResult { IsSuccessful = true, Message = "File " + downloadsPath + blobToDownload + " downloaded!" };
            }
            catch (Exception ex)
            {
                return new DeviceRegistryOperationResult { IsSuccessful = false, Message = ex.Message };
            }
        }

        public async Task<IEnumerable<DeviceRegistryOperationResult>> RegisterBulkDevices(object request)
        {
            List<DeviceRegistryOperationResult> bulkResult = new List<DeviceRegistryOperationResult>();
            try
            {
                #region -- local debugging
                //string path = @"D:\ -SourceCode\BulkUpload\POSTRequest.txt";
                ////string request;
                //using (var streamReader = new StreamReader(path, Encoding.UTF8))
                //{
                //    request = streamReader.ReadToEnd();
                //}
                #endregion -- local debugging

                string devices = Convert.ToString(request);
                devices = devices.TrimStart('\"');
                devices = devices.TrimEnd('\"');
                devices = devices.Replace("\\", "");
                var result = JsonConvert.DeserializeObject<List<IotDevice>>(devices);

                if (result != null)
                {
                    for (int i = 0; i < result.Count(); i++)
                    {
                        // Continue looping for other records, even when exception occurs for one record
                        try
                        {
                            var registerResult = await RegisterDevice(result[i]);
                            bulkResult.Add(registerResult);
                        }
                        catch (Exception ex)
                        {
                            bulkResult.Add(new DeviceRegistryOperationResult { IsSuccessful = false, Message = ex.Message, IotDeviceData = new IotDevice { Id = result[i].Id } });
                        }
                    }
                }
                else
                {
                    bulkResult.Add(new DeviceRegistryOperationResult { IsSuccessful = false, Message = "failed featching request data" });
                }
            }
            catch (Exception ex)
            {
                bulkResult.Add(new DeviceRegistryOperationResult { IsSuccessful = false, Message = ex.Message });
            }

            return bulkResult;
        }

       
    }

}
