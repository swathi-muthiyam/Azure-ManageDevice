// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CDMProvisioningDeviceClient.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Trac.Core.DM.Manager.Extensions;
using Trac.Core.DM.Manager.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Trac.Core.DM.Manager.Services
{
    class CDMProvisioningDeviceClient
    {
        ProvisioningDeviceClient _provClient;
        SecurityProvider _security;
        ILogger<DPSRegistryManager> _logger;

        public CDMProvisioningDeviceClient(ProvisioningDeviceClient provisioningDeviceClient, SecurityProvider security, ILogger<DPSRegistryManager> logger)
        {
            _provClient = provisioningDeviceClient;
            _security = security;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        public async Task<DPSDeviceRegOperationResult> RunAsync(IotDevice deviceInfo)
        {
            var resultRegister = new DPSDeviceRegOperationResult();
            try
            {
                _logger.LogInformation($"RegistrationID = {_security.GetRegistrationID()}");
                Enrollment.VerifyRegistrationIdFormat(_security.GetRegistrationID());

                Console.Write("ProvisioningClient RegisterAsync . . . ");
                _provClient.ProductInfo = deviceInfo.Id;

                ProvisioningRegistrationAdditionalData data = new ProvisioningRegistrationAdditionalData()
                {
                    JsonData = JsonConvert.SerializeObject(deviceInfo.Payload)
                };                                
                
                DeviceRegistrationResult result = await _provClient.RegisterAsync(data).ConfigureAwait(false);
                resultRegister.DeviceRegResult = result;
                _logger.LogInformation($"{result.Status}");
                _logger.LogInformation($"ProvisioningClient AssignedHub: {result.AssignedHub}; DeviceID: {result.DeviceId}");
              
                
                if (result.Status != ProvisioningRegistrationStatusType.Assigned) return resultRegister;

                IAuthenticationMethod auth;
                if (_security is SecurityProviderTpm)
                {
                    Console.WriteLine("Creating TPM DeviceClient authentication.");
                    auth = new DeviceAuthenticationWithTpm(result.DeviceId, _security as SecurityProviderTpm);
                }
                else if (_security is SecurityProviderX509)
                {
                    Console.WriteLine("Creating X509 DeviceClient authentication.");
                    auth = new DeviceAuthenticationWithX509Certificate(result.DeviceId, (_security as SecurityProviderX509).GetAuthenticationCertificate());
                    
                }
                else if (_security is SecurityProviderSymmetricKey)
                {
                    //*Enrollment Id is taken as device id. You dont have to send device if id DPS is used for registering device.
                    Console.WriteLine("Creating Symmetric Key DeviceClient authenication");
                    auth = new DeviceAuthenticationWithRegistrySymmetricKey(result.DeviceId, (_security as SecurityProviderSymmetricKey).GetPrimaryKey());
                    // var byteStr = Convert.ToBase64String(bytes);
                                       
                }
                else
                {
                    throw new NotSupportedException("Unknown authentication type.");
                }
                resultRegister.DeviceKey = ((DeviceAuthenticationWithRegistrySymmetricKey)auth).Key;

                using (DeviceClient iotClient = DeviceClient.Create(result.AssignedHub, auth, TransportType.Amqp))
                {
                    //if (deviceInfo.TrackingAccess.IsUpdate)
                    //{
                    //    var reportedProperties = new TwinCollection
                    //    {
                    //        ["AllowDeviceToTrack"] = new
                    //        {
                    //            AllowToTrack = deviceInfo.TrackingAccess.AllowTracking
                    //        }
                    //    };
                    //    await iotClient.UpdateReportedPropertiesAsync(reportedProperties);

                    //}
                    //else
                    //{
                        _logger.LogInformation("DeviceClient OpenAsync");
                        await iotClient.OpenAsync().ConfigureAwait(false);
                        _logger.LogInformation("DeviceClient SendEventAsync");
                        await iotClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes("Device " + result.DeviceId + " Registered to IOT Hub - " + result.AssignedHub))).ConfigureAwait(false);
                        _logger.LogInformation("DeviceClient CloseAsync");
                        await iotClient.CloseAsync().ConfigureAwait(false);
                    //}
                }

                return resultRegister;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

    }
}
