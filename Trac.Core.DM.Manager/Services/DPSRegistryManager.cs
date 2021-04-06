// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DPSRegistryManager.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Trac.Core.Common.Configuration;
using Trac.Core.DM.Manager.Extensions;
using Trac.Core.DM.Manager.Interfaces;
using Trac.Core.DM.Manager.Models;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Trac.Core.DM.Manager.Services
{
    public class DPSRegistryManager : IDPSRegistryManager
    {
        private readonly ILogger<DPSRegistryManager> _logger;
        public DPSRegistryManager(ILogger<DPSRegistryManager> logger)
        {
            _logger = logger;
            //this._registerConfig.GetSection("DPS:DPS_IDSCOPE");
            //this._registerConfig = KeyVaultStoreManager.keyVaultStoreManagerInstance.GetValueFromKeyVault();
        }

        public async Task<DPSDeviceRegOperationResult> RegisterDeviceAsync(IotDevice iotHubDevice, IDeviceRegistryManager deviceRegistryMgr, IConfiguration _registerConfig)
        {
            //https://github.com/Azure-Samples/azure-iot-samples-csharp/blob/master/provisioning/Samples/device/SymmetricKeySample/ProvisioningDeviceClientSample.cs
            // TODO : Connect to DB to get enrollment configuration specific to customerId and device group ID.
            // Say from UI , User selects Customer, DeviceGroup & Opsts for Group enrollment registration. Then from config Primary key of that should be fetched. Which is pre-saved during deployment process.
            var groupEnrollment = new GroupEnrollmentModel() {  AssestationType = AssestationType.SymmetricKey, SymPrimaryKey = iotHubDevice.Auth };
            var enrollmentSetting = new EnrollmentModel() { EnrollmentName = iotHubDevice.EnrollmentId,EnrollmentType=EnrollmentType.GroupEnrollment,  GroupEnrollmentModel = groupEnrollment};
            enrollmentSetting.GlobalDeviceEndpoint = "global.azure-devices-provisioning.net";
            enrollmentSetting.IdScope = "0ne0024F0A3";

            if (enrollmentSetting.EnrollmentType == EnrollmentType.GroupEnrollment
                && enrollmentSetting.GroupEnrollmentModel.AssestationType == AssestationType.SymmetricKey)
            {               
                groupEnrollment.SymPrimaryKey = groupEnrollment.SymPrimaryKey.ComputeDerivedSymmetricKey(iotHubDevice.Id);
                using var security = new SecurityProviderSymmetricKey(
                iotHubDevice.Id,
                enrollmentSetting.GroupEnrollmentModel.SymPrimaryKey,
                null);
                using var transportHandler = GetTransportHandler(enrollmentSetting);
                // based on enrollment setting iothub 
                ProvisioningDeviceClient provClient = ProvisioningDeviceClient.Create(
                    enrollmentSetting.GlobalDeviceEndpoint,
                    enrollmentSetting.IdScope,
                    security,
                    transportHandler);
                Console.WriteLine($"Initialized for registration Id {security.GetRegistrationID()}.");

                Console.WriteLine("Registering with the device provisioning service...");
                DeviceRegistrationResult result = await provClient.RegisterAsync();

                Console.WriteLine($"Registration status: {result.Status}.");
                if (result.Status != ProvisioningRegistrationStatusType.Assigned)
                {
                    Console.WriteLine($"Registration status did not assign a hub, so exiting this sample.");
                    return new DPSDeviceRegOperationResult() { Message = "Registration status did not assign a hub" };
                }
                Console.WriteLine($"Device {result.DeviceId} registered to {result.AssignedHub}.");

                Console.WriteLine("Creating symmetric key authentication for IoT Hub...");
                IAuthenticationMethod auth = new DeviceAuthenticationWithRegistrySymmetricKey(
                    result.DeviceId,
                    security.GetPrimaryKey());

                Console.WriteLine($"Testing the provisioned device with IoT Hub...");
                using DeviceClient iotClient = DeviceClient.Create(result.AssignedHub, auth, enrollmentSetting.TransportType);
                return new DPSDeviceRegOperationResult() { Message = "Successfully Registered!!" };
            }
            return new DPSDeviceRegOperationResult() { Message = "Registered!!" };
        }

        //public DPSDeviceRegOperationResult RegisterDevice(IotDevice iotHubDevice, IDeviceRegistryManager deviceRegistryMgr, IConfiguration _registerConfig)

        //{
        //    // https://github.com/Azure-Samples/azure-iot-samples-csharp/blob/master/provisioning/Samples/device/SymmetricKeySample/ProvisioningDeviceClientSample.cs
        //    //IEnumerable<IotDevice> deviceCheck = Task.Run(async () => await deviceRegistryMgr.GetDevicesAsync()).Result;

        //    //foreach(IotDevice iot in deviceCheck)
        //    //{
        //    //    if(iot.Id.ToUpper() == iotHubDevice.Id.ToUpper())
        //    //    {
        //    //        return new DeviceRegistryOperationResult
        //    //        {
        //    //            IsSuccessful = true, // This is made true just to let system know device is registered & it needs approval to share telemetry data.
        //    //            Message = "Device Already Registered!"
        //    //        };
        //    //    }
        //    //}

        //    // TODO : Keep the key in database for generic code. TO BE modified. This information should be fetched based on tenent and deviceTwin information
        //    // While Creating hub name make sure to prefix it with same prefix as of device.
        //    string DPS_IDSCOPE = _registerConfig["DPS:DPS_IDSCOPE"];
        //    string GlobalDeviceEndpoint = _registerConfig["DPS:GlobalDeviceEndpoint"];
        //    string registrationId = iotHubDevice.Id;
        //    string individualEnrollmentPrimaryKey = _registerConfig["DPS:individualEnrollmentPrimaryKey"];
        //    string individualEnrollmentSecondaryKey = _registerConfig["DPS:individualEnrollmentSecondaryKey"];
        //    string enrollmentGroupPrimaryKey = _registerConfig["DPS:enrollmentGroupPrimaryKey"];
        //    string enrollmentGroupSecondaryKey = _registerConfig["DPS:enrollmentGroupSecondaryKey"];

        //    if (string.IsNullOrWhiteSpace(DPS_IDSCOPE))
        //    {
        //        _logger.LogError("Missing DPS_IDSCOPE config");
        //        return new DPSDeviceRegOperationResult() { Message = "Missing DPS ID Scope. Verify deployment config" };
        //    }

        //    if (string.IsNullOrWhiteSpace(individualEnrollmentPrimaryKey) && string.IsNullOrWhiteSpace(enrollmentGroupPrimaryKey))
        //    {
        //        _logger.LogError("Missing individualEnrollmentPrimaryKey or inrollmentGroupPrimaryKey config");
        //        return new DPSDeviceRegOperationResult() { Message = "Missing IndividualEnrollmentPrimaryKey or EnrollmentGroupPrimaryKey. Verify deployment config" };
        //    }

        //    string primaryKey, secondaryKey;

        //    if (!String.IsNullOrEmpty(registrationId) && !String.IsNullOrEmpty(enrollmentGroupPrimaryKey) && !String.IsNullOrEmpty(enrollmentGroupSecondaryKey))
        //    {
        //        //Group enrollment flow, the primary and secondary keys are derived from the enrollment group keys and from the desired registration id
        //        primaryKey = Enrollment.ComputeDerivedSymmetricKey(Convert.FromBase64String(enrollmentGroupPrimaryKey), registrationId);
        //        secondaryKey = Enrollment.ComputeDerivedSymmetricKey(Convert.FromBase64String(enrollmentGroupSecondaryKey), registrationId);

        //    }
        //    else if (!String.IsNullOrEmpty(registrationId) && !String.IsNullOrEmpty(individualEnrollmentPrimaryKey) && !String.IsNullOrEmpty(individualEnrollmentSecondaryKey))
        //    {
        //        //Individual enrollment flow, the primary and secondary keys are the same as the individual enrollment keys
        //        primaryKey = individualEnrollmentPrimaryKey;
        //        secondaryKey = individualEnrollmentSecondaryKey;

        //    }
        //    else
        //    {
        //        _logger.LogError("Invalid configuration provided, must provide group enrollment keys or individual enrollment keys");
        //        return new DPSDeviceRegOperationResult() { Message = "Verify individualEnrollmentPrimaryKey config" };
        //    }
        //    using var security = new SecurityProviderSymmetricKey(registrationId, primaryKey, secondaryKey);

        //    // Select one of the available transports:
        //    // To optimize for size, reference only the protocols used by your application.

        //    // using (var transport = new ProvisioningTransportHandlerHttp())
        //    // using (var transport = new ProvisioningTransportHandlerMqtt(TransportFallbackType.TcpOnly))
        //    // using (var transport = new ProvisioningTransportHandlerMqtt(TransportFallbackType.WebSocketOnly))
        //    using var transport = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly);
        //    ProvisioningDeviceClient provClient =
        //      ProvisioningDeviceClient.Create(GlobalDeviceEndpoint, DPS_IDSCOPE, security, transport);

        //    var client = new CDMProvisioningDeviceClient(provClient, security, _logger);

        //    var result = client.RunAsync(iotHubDevice).GetAwaiter().GetResult();

        //    if (result.DeviceRegResult.Status == ProvisioningRegistrationStatusType.Assigned)
        //    {
        //        return new DPSDeviceRegOperationResult() { DeviceRegResult = result.DeviceRegResult, DeviceKey = result.DeviceKey, Message = $"Successfully registered the device: {result.DeviceRegResult.DeviceId}" };
        //    }
        //    return new DPSDeviceRegOperationResult() { DeviceRegResult = result.DeviceRegResult, DeviceKey = result.DeviceKey, Message = result.DeviceRegResult.ErrorMessage };

        //}

        public DPSDeviceRegOperationResult GetRegisteredDevices(IotDevice iotHubDevice, IDeviceRegistryManager deviceRegistryMgr, IConfiguration _registerConfig)
        {
            return new DPSDeviceRegOperationResult();
        }

        private ProvisioningTransportHandler GetTransportHandler(EnrollmentModel enrollmentModel)
        {
            return enrollmentModel.TransportType switch 
            {
                //TransportType.Mqtt => new ProvisioningTransportHandlerMqtt(),
                //TransportType.Mqtt_Tcp_Only => new ProvisioningTransportHandlerMqtt(TransportFallbackType.WebSocketOnly),
                //TransportType.Mqtt_WebSocket_Only => new ProvisioningTransportHandlerMqtt(TransportFallbackType.TcpOnly),
                TransportType.Amqp => new ProvisioningTransportHandlerAmqp(),
                TransportType.Amqp_Tcp_Only => new ProvisioningTransportHandlerAmqp(TransportFallbackType.WebSocketOnly),
                TransportType.Amqp_WebSocket_Only => new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly),
                //TransportType.Http1 => new ProvisioningTransportHandlerHttp(),
                _ => throw new NotSupportedException($"Unsupported transport type {enrollmentModel.TransportType}"),
            };
        }



    }
}
