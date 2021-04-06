
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iothubtest
{
    class Program
    {
        // "{iot hub hostname}"
        private const string connectionString = "HostName=registrationiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=6peIIFZY640XivxH7BUk7ySF36DVGCpW7wuRyqv3Fx4=";
        private const string iotHubD2cEndpoint = "messages/events";

        private const string IotHubUri = "registrationiothub.azure-devices.net";//"swatiothub.azure-devices.net";
        private const string DeviceKey = "GEyxG23Sfv7vV6Gg6E5XzwfMC/yubkazfgG9qk2EERU="; //"{device key}"; 
        //HostName=registrationiothub.azure-devices.net;DeviceId=c1b2f3w4-001;SharedAccessKey=GEyxG23Sfv7vV6Gg6E5XzwfMC/yubkazfgG9qk2EERU=
        private const string DeviceIdsimulator = "c1b2f3w4-001";//"campus1-buil1-f2-l2-99";//"myFirstDevice";
        private const double MinTemperature = 20;
        private const double MinHumidity = 60;
        private static readonly Random Rand = new Random();
        private static DeviceClient _deviceClient;
        private static int _messageId = 1;
        public static void Main(string[] args)
        {
            //Random rand = new Random();
            //for (int i = 0; i < 10; i++)
            //{
            //    string currentTemperature = Convert.ToString(11 + rand.Next() * 5 - 3);
            //}

            #region "Working register device, send message device to cloud..."
            //Console.WriteLine("Simulated device\n");
            //string DeviceId = "c1b2f3w4" + Convert.ToString(Rand.Next());
            //Console.WriteLine("look for device "+ DeviceId + " \n");
            //CreateDeviceIdentity("");

            SimulateDeviceToSendD2CAndReceiveD2C();
            #endregion

            Console.ReadLine();
        }

        private static void CreateDeviceIdentity(string deviceName)
        {
            AzureIoTHub.CreateDeviceIdentityAsync(deviceName).Wait();
            Console.WriteLine($"Device with name '{deviceName}' was created/retrieved successfully");
        }
        private static void SimulateDeviceToSendD2CAndReceiveD2C()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource(6000);

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                tokenSource.Cancel();
                Console.WriteLine("Exiting ...");
            };
            Console.WriteLine("Press CTRL+C to exit");

            //Task.WaitAll(
            //    AzureIoTHub.SendDeviceToCloudMessageAsync(tokenSource.Token)
            //    //,AzureIoTHub.ReceiveMessagesFromDeviceAsync(tokenSource.Token)
            //    );

            Task.WaitAll(
            AzureIoTHub.XSendDeviceToCloudMessageAsync()

               );

             //Meter Reading
            Task.WaitAll(
              AzureIoTHub.MeterSendDeviceToCloudMessageAsync()

              );
        }

        private static async void XSendDeviceToCloudMessagesAsync()
        {
            while (true)
            {
                var currentTemperature = MinTemperature + Rand.NextDouble() * 15;
                var currentHumidity = MinHumidity + Rand.NextDouble() * 20;

                var telemetryDataPoint = new
                {
                    messageId = _messageId++,
                    deviceId = DeviceIdsimulator,
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("Temperature", (currentTemperature > 30) ? "true" : "false");

                await _deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }
        }




    }
}
