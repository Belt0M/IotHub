using Microsoft.Azure.Devices.Client;

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
    }
}
