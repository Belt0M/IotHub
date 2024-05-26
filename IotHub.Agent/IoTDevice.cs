using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
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

            await UpdateTwinAsync(opcManager.client);
        }

        #endregion

        #region Direct Methods
        private async Task<MethodResponse> EmergencyStopHandler(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\t METHOD EXECUTED: {methodRequest.Name}");

            await opcManager.EmergencyStop();

            return new MethodResponse(0);
        }

        private async Task<MethodResponse> ResetErrorStatusHandler(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\t METHOD EXECUTED: {methodRequest.Name}");

            await opcManager.ResetErrorStatus();

            return new MethodResponse(0);
        }

        private async Task<MethodResponse> DefaultServiceHandler(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\t DEFAULT METHOD EXECUTED: {methodRequest.Name}");

            await Task.Delay(1000);

            return new MethodResponse(0);
        }
        #endregion

        #region Device Twin

        public async Task UpdateTwinAsync(OpcClient opcClient)
        {
            StringBuilder errorBuilder = new StringBuilder();

            int errors = (int)opcClient.ReadNode($"ns=2;s=Device {Program.deviceNumber}/DeviceError").Value;

            if ((errors & Convert.ToInt32(Errors.Unknown)) != 0)
            {
                errorBuilder.Append("Unknown ");
            }

            if ((errors & Convert.ToInt32(Errors.SensorFailue)) != 0)
            {
                errorBuilder.Append("SensorFailure ");
            }

            if ((errors & Convert.ToInt32(Errors.PowerFailure)) != 0)
            {
                errorBuilder.Append("PowerFailure ");
            }

            if ((errors & Convert.ToInt32(Errors.EmergencyStop)) != 0)
            {
                errorBuilder.Append("Emergency stop ");
            }

            string errorsString = errorBuilder.ToString();

            var reportedProperties = new TwinCollection();

            reportedProperties["productionRate"] = opcClient.ReadNode($"ns=2;s=Device {Program.deviceNumber}/ProductionRate").Value;
            reportedProperties["deviceErrors"] = errorsString;

            await client.UpdateReportedPropertiesAsync(reportedProperties);
        }

        private async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            int value = desiredProperties["productionRate"];

            await opcManager.SetProductionRate(value);

            TwinCollection reportedProperties = new TwinCollection();

            reportedProperties["productionRate"] = value;

            await client.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
        }

        #endregion

        public async Task InitializeHandlers()
        {
            await client.SetMethodHandlerAsync("EmergencyStop", EmergencyStopHandler, client);

            await client.SetMethodHandlerAsync("ResetErrorStatus", ResetErrorStatusHandler, client);

            await client.SetMethodDefaultHandlerAsync(DefaultServiceHandler, client);

            await client.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged, client);
        }
    }
}
