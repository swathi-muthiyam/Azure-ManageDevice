//using Azure;
using Microsoft.Azure;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.ServiceBus.Messaging;
//using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Devices = Microsoft.Azure.Devices;

namespace iothubtest
{
    public static class AzureIoTHub
    {
        /// <summary>
        /// Please replace with correct connection string value
        /// The connection string could be got from Azure IoT Hub -> Shared access policies -> iothubowner -> Connection String:
        /// </summary>
        //private const string iothubconnectionString = "HostName=iothubtempsensor.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=2mRT7OM/E7FmzoVhWnRbf+hebYRhTddbumlb6lVe+jI=";

        // Meter Reading
        private const string iothubconnectionString = "HostName=iothubMeterReading.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=rKn1GxrkhHxavHEFfMy1fsF3l3/39Z05rPHBVhL7y5w=";

        /// <summary>
        /// Please replace with correct device connection string
        /// The device connect string could be got from Azure IoT Hub -> Devices -> {your device name } -> Connection string
        /// </summary>
       // private const string deviceConnectionString = "HostName=iothubtempsensor.azure-devices.net;DeviceId=test1;SharedAccessKey=nGuVV8O4DCjKaQ9PKI3veP76F44+YrJhu6xssPe+eds=";
        // Meter Reading
        private const string deviceConnectionString = "HostName=iothubMeterReading.azure-devices.net;DeviceId=MeterLive;SharedAccessKey=LO9dQK4eWi1uSqcGGOKdm2o2pjUpCUhEssgHU/JpyJk=";

        // private const string deviceConnectionString = "HostName=registrationiothub.azure-devices.net;DeviceId=ParkingSlot-fa37221d-aefb-422e-87bc-4574dd7e9727;SharedAccessKey=S3Qw1km9jf9tbdHILNQwUcu952Z7SVoGEn30owVwtUQ=";
        //private const string deviceConnectionString = "HostName=registrationiothub.azure-devices.net;DeviceId=Device001;SharedAccessKey=TMTbi8JtnkxFwifsrmWAr69OcQKqfmuEtUlVLzku1pY=";

        private const string iotHubD2cEndpoint = "messages/events";

        public static async Task<string> CreateDeviceIdentityAsync(string deviceName)
        {
            var registryManager = Devices.RegistryManager.CreateFromConnectionString(iothubconnectionString);
            Devices.Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Devices.Device(deviceName));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceName);
            }
            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        public static async Task SendDeviceToCloudMessageAsync(CancellationToken cancelToken)
        {
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);
            //double avgTemperature = 70; // m/s
            Random rand = new Random();
            // string SlotStatus = "Occupied";
            int slotId = 0;
            //while (true)
            //{
            //if (cancelToken.IsCancellationRequested)
            //    break;
            slotId = slotId > 3 ? slotId = 1 : ++slotId;
            //double currentTemperature = avgTemperature + rand.NextDouble() * 4 - 3;
            //SlotStatus = !(SlotStatus == "Occupied") ? "Occupied" : "Available";
            var telemetryDataPoint = new
            {
                //ParkingSlotId = "febacf4d-7f33-430b-8d6f-06271080c5ee",
                SlotStatus = "Occupied"   // Occupied , 

            };
            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));
            await deviceClient.SendEventAsync(message);
            Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
            await Task.Delay(5000);
            //}
        }



        internal static async Task<Task[]> XSendDeviceToCloudMessageAsync()
        {
            Random rand = new Random();
            int currentTemperature = 11 + rand.Next() * 5 - 3;
            int _messageId = 1;
            string deviceId = "test1";

            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);
            while (true)
            {
                 currentTemperature = currentTemperature + rand.Next() * 15;
                var currentHumidity = currentTemperature + rand.NextDouble() * 20;

                var telemetryDataPoint = new
                {
                    messageId = _messageId++,
                    deviceId = deviceId,
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");
                message.Properties.Add("condition", Convert.ToString(telemetryDataPoint));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }
        }

        internal static async Task<Task[]> MeterSendDeviceToCloudMessageAsync()
        {
            Random rand = new Random();
            int meterReading = 11 + rand.Next() * 5 - 3;
            int _messageId = 1;
            string deviceId = "MeterLive";

            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);
            while (true)
            {
               

                var telemetryDataPoint = new
                {
                    messageId = _messageId++,
                    deviceId = deviceId,
                    MeterReading = meterReading,
                  
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                //message.Properties.Add("temperatureAlert", (meterReading > 30) ? "true" : "false");
                message.Properties.Add("MeterLive", Convert.ToString(telemetryDataPoint));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }
        }

        public static async Task<string> ReceiveCloudToDeviceMessageAsync()
        {
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);

            while (true)
            {
                var receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    await deviceClient.CompleteAsync(receivedMessage);
                    return messageData;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public static async Task ReceiveMessagesFromDeviceAsync(CancellationToken cancelToken)
        {
            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(iothubconnectionString, iotHubD2cEndpoint);
            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            await Task.WhenAll(d2cPartitions.Select(partition => ReceiveMessagesFromDeviceAsync(eventHubClient, partition, cancelToken)));
        }

        private static async Task ReceiveMessagesFromDeviceAsync(EventHubClient eventHubClient, string partition, CancellationToken ct)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
            while (true)
            {
                if (ct.IsCancellationRequested)
                    break;

                EventData eventData = await eventHubReceiver.ReceiveAsync(TimeSpan.FromSeconds(2));
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received. Partition: {0} Data: '{1}'", partition, data);
            }
        }

        #region
        #endregion
    }
}
