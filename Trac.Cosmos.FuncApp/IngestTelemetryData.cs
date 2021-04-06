// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IngestTelemetryData.cs" company=" "> 
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trac.Cosmos.FuncApp.Interfaces;
using Trac.Cosmos.FuncApp.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Trac.Cosmos.FuncApp
{
    public class IngestTelemetryData
    {
        #region SQL
        private readonly ICosmosSQLService _cosmosDbService;
        public IngestTelemetryData(ICosmosSQLService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }
        #endregion SQL
        #region Mongo
        //private readonly ICosmosMongoService _cosmosDbService;

        //public IngestTelemetryData(ICosmosMongoService cosmosDbService)
        //{
        //    _cosmosDbService = cosmosDbService;
        //}

        #endregion Mongo

        [FunctionName("IngestTelemetryData")]
        public async Task Run([EventHubTrigger("samples-workitems", Connection = "EventHubConnectionAppSetting")] EventData[] events, ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    var sysProp = new SysProperties()
                    {
                        DeviceId = Convert.ToString(eventData.SystemProperties["iothub-connection-device-id"]),
                        AuthGenerationID = Convert.ToString(eventData.SystemProperties["iothub-connection-auth-generation-id"]),
                        AuthMethod = Convert.ToString(eventData.SystemProperties["iothub-connection-auth-method"]),
                        EnqueuedTime = Convert.ToString(eventData.SystemProperties["iothub-enqueuedtime"]),
                        MessageSource = Convert.ToString(eventData.SystemProperties["iothub-message-source"])
                    };

                    //string msgBody = Encoding.UTF8.GetString(eventData.Body).TrimStart('[').TrimEnd(']');
                    ////msgBody = msgBody.Remove(msgBody.Length, 1);
                    //JObject job = (JObject)JsonConvert.DeserializeObject(msgBody);
                    //var result = new Root();
                    //var telemetryBody = string.Empty;
                    //foreach (var obj in job)
                    //{
                    //    if(obj.Key.ToLower().Equals("data"))
                    //    { 
                    //        result = JsonConvert.DeserializeObject<Root>(obj.Value.ToString());
                    //        byte[] body = System.Convert.FromBase64String(Convert.ToString(result.body));
                    //        var value = System.Text.ASCIIEncoding.ASCII.GetString(body);
                    //        telemetryBody =Convert.ToString((JObject)JsonConvert.DeserializeObject(value));
                    //        //telemetryBody = JsonConvert.DeserializeObject<DeviceTelemetryModel>(bodyProperty.ToString());

                    //    }
                    //}
                    //log.LogInformation($"job Object: {job.Properties()}");
                    ////string jSon = new StreamReader(eventData.Body.ToString()).ReadToEnd();
                    ////string deviceId = (string)job["systemProperties"]["iothub-connection-device-id"];
                    /////log.LogInformation($"Found device: {deviceId}");

                    #region -- SQL API--
                    string partKey = DateTime.Now.ToString("MMddyyyyhhmmss");
                    
                        var tele = new DeviceTelemetry()
                        {
                            deviceId = sysProp.DeviceId + "-" + partKey,
                            MessageBody = messageBody,                            
                            SystemProperty = sysProp
                        };
                        var _re = _cosmosDbService.AddTelemetryAsync(tele);
                    #endregion -- SQL API--

                    #region -- MONGO API --
                    //var tele = new MDeviceTelemetry() { MessageBody = messageBody, SystemProperty = sysProp };
                    //_cosmosDbService.CreateTask(tele);

                    #endregion --MONGO API --
                    // Replace these two lines with your processing logic.
                    log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
