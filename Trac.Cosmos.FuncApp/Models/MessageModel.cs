using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trac.Cosmos.FuncApp.Models
{
   
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Properties
        {
            public string temperatureAlert { get; set; }
        }

        public class SystemProperties
        {
            [JsonProperty("iothub-connection-device-id")]
            public string IothubConnectionDeviceId { get; set; }

            [JsonProperty("iothub-connection-auth-method")]
            public string IothubConnectionAuthMethod { get; set; }

            [JsonProperty("iothub-connection-auth-generation-id")]
            public string IothubConnectionAuthGenerationId { get; set; }

            [JsonProperty("iothub-enqueuedtime")]
            public DateTime IothubEnqueuedtime { get; set; }

            [JsonProperty("iothub-message-source")]
            public string IothubMessageSource { get; set; }
        }

        public class Root
        {
            public Properties properties { get; set; }
            public SystemProperties systemProperties { get; set; }
            public string body { get; set; }
        }


    }

