using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Opc.UaFx.Client;
using System.Net.Mime;
using System.Text;

namespace IndustrialIoT
{
    enum Errors
    {
        EmergencyStop = 1,
        PowerFailure = 2,
        SensorFailue = 4,
        Unknown = 8
    }

    public class IoTDevice
    {
        private readonly DeviceClient client;
        private OpcManager opcManager;

        public IoTDevice(DeviceClient deviceClient, OpcManager opcManager)
        {
            client = deviceClient;
            this.opcManager = opcManager;
        }


        #region Sending Messages

        public async Task SendMessages(OpcClient opcClient, int deviceNumber)
        {
            var data = new
            {
                productionStatus = opcClient.ReadNode($"ns=2;s=Device {deviceNumber}/ProductionStatus").Value,
                workorderId = opcClient.ReadNode($"ns=2;s=Device {deviceNumber}/WorkorderId").Value,
                goodCount = opcClient.ReadNode($"ns=2;s=Device {deviceNumber}/GoodCount").Value,
                badCount = opcClient.ReadNode($"ns=2;s=Device {deviceNumber}/BadCount").Value,
                temperature = opcClient.ReadNode($"ns=2;s=Device {deviceNumber}/Temperature").Value
            };

            var dataString = JsonConvert.SerializeObject(data);

            Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataString));

            eventMessage.ContentType = MediaTypeNames.Application.Json;

            eventMessage.ContentEncoding = "utf-8";

            await client.SendEventAsync(eventMessage);
        }

        #endregion
    }
}
