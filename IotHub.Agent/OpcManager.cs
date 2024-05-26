using Opc.UaFx;
using Opc.UaFx.Client;

namespace IndustrialIoT
{
    public class OpcManager
    {
        public OpcClient client;

        public OpcManager(string opcConnectionString)
        {
            client = new OpcClient(opcConnectionString);
        }

        public void Start()
        {
            // Console.WriteLine(File.ReadAllLines(currentPath + "/Config.txt")[1].ToString());
            client.Connect();

            Console.WriteLine("Device list:");

            var node = client.BrowseNode(OpcObjectTypes.ObjectsFolder);

            foreach (var childNode in node.Children())
                if (!childNode.DisplayName.Value.Contains("Server"))
                {
                    Console.WriteLine($"{childNode.Name}");
                }
        }

        public async Task EmergencyStop()
        {
            client.CallMethod($"ns=2;s=Device {Program.deviceNumber}", $"ns=2;s=Device {Program.deviceNumber}/EmergencyStop");

            await Task.Delay(1000);
        }

        public async Task ResetErrorStatus()
        {
            client.CallMethod($"ns=2;s=Device {Program.deviceNumber}", $"ns=2;s=Device {Program.deviceNumber}/ResetErrorStatus");

            await Task.Delay(1000);
        }
        public async Task SetProductionRate(int value)
        {
            client.WriteNode($"ns=2;s=Device {Program.deviceNumber}/ProductionRate", value);

            await Task.Delay(1000);
        }

    }
}