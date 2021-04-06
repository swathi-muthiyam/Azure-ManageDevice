// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceService.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.DigitalTwins.Core;
using Azure.DigitalTwins.Core.Serialization;
using Azure.Identity;
using Trac.Core.ADT.Common.Models;
using Trac.Core.ADT.Common.ViewModels;
using Trac.Core.ADT.Manager.Constants;
using Trac.Core.ADT.Manager.Interfaces;
using Trac.Core.ADT.Manager.Models;
using Trac.Core.Common.Configuration;
using Trac.Core.Common.Configuration.Models;
using Trac.Core.DigitalTwin.Manager;
using Microsoft.Extensions.Options;

namespace Trac.Core.ADT.Manager
{
    public class DeviceService : IDeviceService
    {
        private readonly ADTCredentials adtCredentials;
        private DigitalTwinsClient _digitalTwinsClient;
        private IADTClientManager _adtClientManager;
        private readonly Random _random = new Random();

        //public DeviceService(IOptions<ADTCredentials> adtCredentialsOptions)
        //{
        //    this.adtCredentials = adtCredentialsOptions.Value;
        //    CreateAdtClient();
        //}

        public DeviceService()
        {
            
            if (_digitalTwinsClient == null)
            {
                var adtSecrets = KeyVaultStoreManager.keyVaultStoreManagerInstance.GetValueFromKeyVault();
                var credentials = new ClientSecretCredential(adtSecrets[ADTConstants.tenentId], adtSecrets[ADTConstants.clientId], adtSecrets[ADTConstants.clientSecret]);
                this._digitalTwinsClient = new DigitalTwinsClient(new Uri(adtSecrets[ADTConstants.instanceUrl]), credentials);
                _adtClientManager = new ADTClientManager(_digitalTwinsClient);
            }
        }

        private void CreateAdtClient()
        {
            var credential = new ClientSecretCredential(adtCredentials.TenantId, adtCredentials.ClientId, adtCredentials.ClientSecret);
            _digitalTwinsClient = new DigitalTwinsClient(new Uri(adtCredentials.AdtInstanceUrl), credential);
            _adtClientManager = new ADTClientManager(_digitalTwinsClient);
        }

        public async Task<bool> RegisterDeviceInAdt(GroundSensorDeviceVM groundSensorDeviceVm)
        {
            GroundSensorDevice device = new GroundSensorDevice(){ DeviceId = groundSensorDeviceVm.DeviceId, Status = groundSensorDeviceVm.Status, ETag = groundSensorDeviceVm.ETag };
            BasicDigitalTwin deviceDigitalTwin = CreateDeviceDigitalTwin(device);

            try
            {
                await _adtClientManager.CreateDigitalTwin(deviceDigitalTwin);

                //Map device to space
                CreateRelationship relationshipObj = new CreateRelationship()
                {
                    SourceTwinId = $"ParkingSlot-{groundSensorDeviceVm.ParkingSlotId}",
                    TargetTwinId = $"Device-{groundSensorDeviceVm.DeviceId}",
                    RelationshipName = new ParkingSlot().Relationship,
                    RelationshipId = $"slotDeviceRelationship{_random.Next()}"
                };

                await _adtClientManager.CreateRelationship(relationshipObj);
            }
            catch (Exception e)
            {
                return false;
            }

            return await Task.FromResult(true);
        }

        private BasicDigitalTwin CreateDeviceDigitalTwin(GroundSensorDevice device)
        {
            // Initialize twin data
            BasicDigitalTwin twinData = new BasicDigitalTwin();
            twinData.Metadata.ModelId = device.IdDef;
            twinData.Id = $"Device-{device.DeviceId}";

            twinData.CustomProperties.Add(nameof(device.DeviceId), device.DeviceId);
            twinData.CustomProperties.Add(nameof(device.ETag), device.ETag);
            twinData.CustomProperties.Add(nameof(device.Status), device.Status.ToString());


            return twinData;
        }
    }
}
