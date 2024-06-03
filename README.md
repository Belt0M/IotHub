# IoT Project

This documentation provides instructions for connecting, adding new devices, executing methods, and filtering data within an agent that connects simulated industrial devices on an OPC server and Azure IoTHub

### To start working with an agent, you need to have:

- Azure IoT Hub
- Downloaded IIoTSim to simulate devices and D2C messages
- Azure Eplorer for easier work with data received from devices
- Cloned this repository or downloaded archive
- Download and install the C# SDK from [here](https://dotnet.microsoft.com/en-us/download).

## 1. Agent Configuration

### Firstly, you need to create the required number of devices in the simulator, for this:

- In the project folder there is an archive with the simulator (IIoTSim.zip), unzip it to any place on the computer and open the "setup.exe" file
- Use the "New device" button to create the required number of devices/enterprise lines
- Select any device and increase the "Production Rate" using the plus button
- Start the device with the "Start" button

The devices in the simulator are ready and configured

### Now you need to create the corresponding devices in Azure IoTHub

You need to go to your IoTHub or create one if you don't already have one, and then:

 - Go to the "Device management" section -> "Devices"
 - Click the "Add device" button
 - Enter the name of the device and choose the desired settings (or leave the default if you don't know what to choose)
 - Click the "Save" button

Repeat the steps for the required number of devices and you're done

### Now you need to enter data about the created devices in the agent configuration file

For this:

- Open the "config.json" file in the root of the project
- Open the desired created device in IoTHub
- Copy "Primary connection string"
- Insert it into the "azureDeviceId" attribute of the device object of the "DEVICES" array in the open pre-configuration file
- If necessary, duplicate the array object and insert the values ​​of other devices

## 2. Run The Application

- Run the project using the green "Run" button at the top of the toolbar

Previously configured devices will appear for selection in the open console window

- Select the desired device by entering its serial number from the list to start sending D2C messages

## 3. Monitoring Device Telemetry

The project folder contains the archive "Azure.IoT.Explorer.Preview.0.15.8.zip"

- Unzip it and open the single file in it

This is an analogue of the web Azure portal, only somewhat simplified and more convenient for monitoring the agent's work

- Log in to your account
- Go to the list of all devices
- Select the running device
- Open the "Telemetry" section

### Here you can track all D2C messages that arrive at this moment or have already arrived earlier

## 4. Monitoring Device Twin Data

- Open the "Device twin" section

### Here you can track all device twin properties and you can add tags and desired properties to your device twin here.
**Objective**: To remove a tag or desired property, set the value of the item to be removed to 'null'.

To change the "production rate" of the device, you need

- Add a field called "prod_rate" and the required value from 0 to 100 in the "desired" field
- Click the "Save" button
- Click the "Reload" button

Now you can see the changed parameter in the "reported" field

## 5. Performing Device Direct Methods

- Open the "Direct method" section
- Enter the name of one of the methods expected below in the "Method name" field
- Click the "Invoke method" button

### Available methods:

- **EmergencyStop**: Emergency stops the device
- **ResetErrorStatus**: Resets all device error statuses

After applying the method, you can observe the result either in device twin or in the devices simulator

## 6. Performing Data Calculations

Data calculations are performed using Azure Stream Analytics Jobs. To start you need:

- Create "Storage account"
- Create 2 blob containers (for example: "temparature", "production _kpi ")
- Create "Stream Analytics Job"
- Go to the newly created job
- Under "Job topology" select "Inputs" and add your "IoTHub" as a data source for analysis
- Similarly, add recently created blob containers as places to save analysis results (in "Outputs" subsection)
- In the Query subsection, insert the code of the desired request, see below

Below are the steps and queries for each calculation type

### 1. Production KPIs

**Objective**: Calculate the percentage of good production in total volume, grouped by device in 5-minute windows.

### Query

```sql
SELECT
    workorderid as WorkOrderID,
    System.Timestamp AS WindowEnd,
    SUM(goodCount) * 100.0 / (SUM(goodCount) + SUM(badCount)) AS GoodProductionPercentage
INTO
    [production-kpi]
FROM
    [IoT-Lato-2024]
GROUP BY
    workorderid,
    TumblingWindow(minute, 5)
```

### Instruction

- **production-kpi**: If you have named your Blob container differently than in the example, then insert its name instead of the given one
- **IoT-Lato-2024**: Provide a name of your IoT Hub

### 2. Temperature

**Objective**: Every minute, calculate the average, minimum, and maximum temperature over the last 5 minutes, grouped by device.

### Query

```sql
SELECT
    workorderid as WorkOrderID,
    System.Timestamp AS WindowEnd,
    AVG(temperature) AS AvgTemperature,
    MIN(temperature) AS MinTemperature,
    MAX(temperature) AS MaxTemperature
INTO
    [temperature]
FROM
    [IoT-Lato-2024]
GROUP BY
    workorderid,
    HoppingWindow(minute,5,1)
```

### Explanation

- **temperature**: If you have named your Blob container differently than in the example, then insert its name instead of the given one
- **IoT-Lato-2024**: Provide a name of your IoT Hub



