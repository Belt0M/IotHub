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
            client.Connect();

            Console.WriteLine("List of available devices for selection: \n");

            var node = client.BrowseNode(OpcObjectTypes.ObjectsFolder);

            foreach (var childNode in node.Children())
                if (!childNode.DisplayName.Value.Contains("Server"))
                {
                    Console.WriteLine($"{childNode.Name}");
                }

            Console.WriteLine("\n");
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