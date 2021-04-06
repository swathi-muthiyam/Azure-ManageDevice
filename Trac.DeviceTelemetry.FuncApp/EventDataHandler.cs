// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventDataHandler.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Trac.Core.ADT.Manager;
using Trac.Core.ADT.Manager.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Trac.Core.DigitalTwin.Manager;
//using Microsoft.Build.Utilities;

namespace Trac.DeviceEvents.FuncApp
{
    public class EventDataHandler 
    {
        private readonly IADTClientManager _adtClientManager;

        public EventDataHandler(IADTClientManager adtClientManager)
        {
            this._adtClientManager = adtClientManager;

        }
        /// <summary>
        /// Trigger to pick up telemetry data from D2C
        /// </summary>
        /// <param name="eventGridEvent"></param>
        /// <param name="log"></param>
        [FunctionName("EventDataHandler")]
        public async void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            try
            {
                if (eventGridEvent != null && eventGridEvent.Data != null)
                {
                    #region Open this region for message format information
                    // Telemetry message format
                    //{
                    //  "properties": { },
                    //  "systemProperties": 
                    // {
                    //    "iothub-connection-device-id": "thermostat1",
                    //    "iothub-connection-auth-method": "{\"scope\":\"device\",\"type\":\"sas\",\"issuer\":\"iothub\",\"acceptingIpFilterRule\":null}",
                    //    "iothub-connection-auth-generation-id": "637199981642612179",
                    //    "iothub-enqueuedtime": "2020-03-18T18:35:08.269Z",
                    //    "iothub-message-source": "Telemetry"
                    //  },
                    //  "body": "eyJUZW1wZXJhdHVyZSI6NzAuOTI3MjM0MDg3MTA1NDg5fQ=="
                    //}
                    #endregion

                    // Reading deviceId from message headers
                    
                    log.LogInformation(eventGridEvent.Data.ToString());
                    JObject job = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
                    log.LogInformation($"job Object: {job.Properties()}");
                    string deviceId = (string)job["systemProperties"]["iothub-connection-device-id"];
                    log.LogInformation($"Found device: {deviceId}");

                    // Extracting slot availibility status from device telemetry
                    byte[] body = System.Convert.FromBase64String(job["body"].ToString());
                    var value = System.Text.ASCIIEncoding.ASCII.GetString(body);
                    var bodyProperty = (JObject)JsonConvert.DeserializeObject(value);
                    var isParkingAvailable = Convert.ToString(bodyProperty["SlotStatus"]);
                    
                    log.LogInformation($"Device Ground Sensor is:{isParkingAvailable}");

                    // Update device slot availibility status property
                    //ADTClientManager _adtClientManager = new ADTClientManager();
                    //await _adtClientManager.UpdateTwinProperty(bodyProperty["SlotId"], "/SlotStatus", isParkingAvailable);
                    // Assuming Device(Ground sensor) generates the event when change in status. 
                    // Find parent using incoming relationships
                    string parentId = await _adtClientManager.FindParent("Device-"+deviceId, "hasGroundSensor");
                    if (parentId != null && !parentId.Contains("Service request failed"))
                    {
                        await _adtClientManager.UpdateTwinProperty(parentId, "/SlotStatus", isParkingAvailable);
                    }

                }
            }
            catch (Exception e)
            {
                log.LogError($"Error in ingest function: {e.Message}");
            }
        }
    }
}
