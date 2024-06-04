using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;

namespace IndustrialIoT
{
    public class Program
    {
        public static int deviceNumber;

        static async Task Main(string[] args)
        {
            var curPath = AppContext.BaseDirectory;

            if (curPath.Contains("\\bin\\Debug\\net6.0\\"))
            {
                curPath = curPath.Split("\\bin\\Debug\\net6.0\\")[0];
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(curPath)
                .AddJsonFile("config.json", optional: false)
                .Build();

            var opcConnectionString = configuration["OPC_CONNECTION_STRING"];

            if (opcConnectionString == null)
            {
                Console.Error.WriteLine("OPC conection string isn't provided");

                return;
            }

            OpcManager opcManager = new(opcConnectionString);

            opcManager.Start();

            var list = configuration.GetSection("DEVICES").GetChildren()
                .Select(d =>
                {
                    var azureDeviceId = d["azureDeviceId"];

                    if (azureDeviceId == null)
                    {
                        Console.Error.WriteLine($"{d} Incorrect devices config data!");
                        return null;
                    }

                    return azureDeviceId;
                })
                .Where(d => d != null).ToList();

            Console.WriteLine("Choose and input a device number from the list above:");

            deviceNumber = Convert.ToInt32(Console.ReadLine());

            if (deviceNumber <= 0 || deviceNumber > list.Count)
            {
                Console.Error.WriteLine("Invalid device number");

                return;
            }

            string deviceConnectionString = list[deviceNumber - 1];

            using var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);

            var iotDevice = new IoTDevice(deviceClient, opcManager);

            Console.WriteLine($"\nConnection successfully established!");

            await iotDevice.InitializeHandlers();

            while (true)
            {
                await iotDevice.SendMessages(opcManager.client, deviceNumber);

                Thread.Sleep(1000);
            }
        }
    }
}