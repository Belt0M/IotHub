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
        bool started = true;
        bool update = false;

        private readonly DeviceClient client;
        private OpcManager opcManager;

        public IoTDevice(DeviceClient deviceClient, OpcManager opcManager)
        {
            client = deviceClient;
            this.opcManager = opcManager;
        }


        #region Sending Messages

        public async Task SendMessages(OpcClient opcClient, int deviceID)
        {
            var objectToSend = new
            {
                productionStatus = opcClient.ReadNode($"ns=2;s=Device {deviceID}/ProductionStatus").Value,
                workorderId = opcClient.ReadNode($"ns=2;s=Device {deviceID}/WorkorderId").Value,
                goodCount = opcClient.ReadNode($"ns=2;s=Device {deviceID}/GoodCount").Value,
                badCount = opcClient.ReadNode($"ns=2;s=Device {deviceID}/BadCount").Value,
                temperature = opcClient.ReadNode($"ns=2;s=Device {deviceID}/Temperature").Value,
                errorsCode = opcClient.ReadNode($"ns=2;s=Device {deviceID}/DeviceError").Value
            };

            var serializedObject = JsonConvert.SerializeObject(objectToSend);

            Message eventMessage = new Message(Encoding.UTF8.GetBytes(serializedObject));

            eventMessage.ContentType = MediaTypeNames.Application.Json;

            eventMessage.ContentEncoding = "utf-8";

            await client.SendEventAsync(eventMessage);

            twinUpdateListener(opcClient);
        }

        #endregion

        #region Direct Methods
        private async Task<MethodResponse> EmergencyStopHandler(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\t {methodRequest.Name} direct method was executed!");

            await opcManager.EmergencyStop();

            return new MethodResponse(0);
        }

        private async Task<MethodResponse> ResetErrorStatusHandler(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\t {methodRequest.Name} direct method was executed!");

            await opcManager.ResetErrorStatus();

            return new MethodResponse(0);
        }

        private async Task<MethodResponse> DefaultServiceHandler(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\t Default direct method was executed: {methodRequest.Name}");

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

            reportedProperties["prodRate"] = opcClient.ReadNode($"ns=2;s=Device {Program.deviceNumber}/ProductionRate").Value;
            reportedProperties["errorsList"] = errorsString;
            reportedProperties["errorsCode"] = errors;

            await client.UpdateReportedPropertiesAsync(reportedProperties);
        }

        private async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            int value = desiredProperties["prodRate"];

            await opcManager.SetProductionRate(value);

            TwinCollection reportedProperties = new TwinCollection();

            reportedProperties["prodRate"] = value;

            await client.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
        }

        #endregion

        #region UpdateTwinOnChange
        public async void twinUpdateListener(OpcClient opcClient)
        {
            if (started)
            {
                await UpdateTwinAsync(opcManager.client);

                started = false;

                Console.Write("Device startup twin configuration was established.");
            }
            else
            {
                Twin twin = await client.GetTwinAsync();

                int clientValue = (int)opcClient.ReadNode($"ns=2;s=Device {Program.deviceNumber}/ProductionRate").Value;
                int twinValue = twin.Properties.Reported["prodRate"];

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
                string twinError = twin.Properties.Reported["errorsList"];

                if (twinValue != clientValue)
                {
                    await UpdateTwinAsync(opcManager.client);

                    Console.WriteLine("ProductionRate Update");
                }

                if (twinError != errorsString)
                {
                    await UpdateTwinAsync(opcManager.client);

                    Console.WriteLine("Error Update");
                }

            }
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
