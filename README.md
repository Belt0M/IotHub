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

![Screenshot_1](https://github.com/Belt0M/IotHub/assets/89530659/4f785222-e7d3-40ca-8f4b-7324a86334c9)
![Screenshot_2](https://github.com/Belt0M/IotHub/assets/89530659/0bbb378d-64b8-473a-9b7b-708ddb7a4f3a)
![Screenshot_3](https://github.com/Belt0M/IotHub/assets/89530659/dafe7acf-a0e9-472e-9022-813e868bf1a9)


The devices in the simulator are ready and configured

### Now you need to create the corresponding devices in Azure IoTHub

You need to go to your IoTHub or create one if you don't already have one, and then:

 - Go to the "Device management" section -> "Devices"
 - Click the "Add device" button
 - Enter the name of the device and choose the desired settings (or leave the default if you don't know what to choose)
 - Click the "Save" button

![Screenshot_4](https://github.com/Belt0M/IotHub/assets/89530659/26e90435-fed0-406c-9336-610a00194eab)
![Screenshot_5](https://github.com/Belt0M/IotHub/assets/89530659/0b794280-ead4-4ee1-af3b-2b8ceae63741)
![Screenshot_6](https://github.com/Belt0M/IotHub/assets/89530659/1af49280-116e-4fb6-adc5-29f8ee2657e1)


Repeat the steps for the required number of devices and you're done

### Now you need to enter data about the created devices in the agent configuration file

For this:

- Open the "config.json" file in the root of the project
- Open the desired created device in IoTHub
- Copy "Primary connection string"
- Insert it into the "azureDeviceId" attribute of the device object of the "DEVICES" array in the open pre-configuration file
- If necessary, duplicate the array object and insert the values ​​of other devices

![Screenshot_8](https://github.com/Belt0M/IotHub/assets/89530659/e2227d26-c5f7-4873-8af6-55c60aaf9472)
![Screenshot_7](https://github.com/Belt0M/IotHub/assets/89530659/ef3f58ce-7f5b-4e39-bef3-270e49763870)

## 2. Run The Application

- Run the project using the green "Run" button at the top of the toolbar

Previously configured devices will appear for selection in the open console window

- Select the desired device by entering its serial number from the list to start sending D2C messages

![Screenshot_10](https://github.com/Belt0M/IotHub/assets/89530659/01e450d4-b161-4584-a9b9-dafb2e3ec877)
![Screenshot_9](https://github.com/Belt0M/IotHub/assets/89530659/a60b861e-fce6-4baa-bfb7-458667b01875)

### The agent will then send a D2C message to IoTHub every second, in the following format:

```
{
  "body": {
    "productionStatus": 1,
    "workorderId": "51bd586f-10f1-47b8-be74-8a03cb11e785",
    "goodCount": 37,
    "badCount": 3,
    "temperature": 65.48090984080524,
    "errorsCode": 0
  },
  "enqueuedTime": "Tue Jun 04 2024 01:20:52 GMT+0200 (за центральноєвропейським літнім часом)"
}
```

## 3. Monitoring Device Telemetry

To work with data from the agent more easily, you need to download Azure Explorer

This is an analogue of the web Azure portal, only somewhat simplified and more convenient for monitoring the agent's work

- Log in to your account
- Go to the list of all devices
- Select the running device
- Open the "Telemetry" section
- Click the "Start" button to start listening a data

![Screenshot_11](https://github.com/Belt0M/IotHub/assets/89530659/d8d6fe24-b17c-4a30-89f3-c501689adee8)
![Screenshot_12](https://github.com/Belt0M/IotHub/assets/89530659/931d0eca-0a12-450e-9a93-7b204d51f1aa)
![Screenshot_13](https://github.com/Belt0M/IotHub/assets/89530659/f151d00b-f424-46e0-94aa-abc207f7c1ce)
![Screenshot_14](https://github.com/Belt0M/IotHub/assets/89530659/3cf45c9f-0b02-4f78-bc87-5d0409b65a9a)
![Screenshot_15](https://github.com/Belt0M/IotHub/assets/89530659/0957a304-ed9c-4357-bc90-4dd405e9e352)

### Here you can track all D2C messages that arrive at this moment or have already arrived earlier

## 4. Monitoring Device Twin Data

- Open the "Device twin" section

### Here you can track all device twin properties and you can add tags and desired properties to your device twin here.
**Objective**: To remove a tag or desired property, set the value of the item to be removed to 'null'.

### Here is an example of the content of a device twin:

```
{
	"deviceId": "device_1",
	"etag": "AAAAAAAAAAs=",
	"deviceEtag": "MTcwOTU4NTg3",
	"status": "enabled",
	"statusUpdateTime": "0001-01-01T00:00:00Z",
	"connectionState": "Disconnected",
	"lastActivityTime": "2024-06-03T23:24:42.0536339Z",
	"cloudToDeviceMessageCount": 0,
	"authenticationType": "sas",
	"x509Thumbprint": {
		"primaryThumbprint": null,
		"secondaryThumbprint": null
	},
	"modelId": "",
	"version": 927,
	"properties": {
		"desired": {
			"prodRate": 20,
			"$metadata": {
				"$lastUpdated": "2024-06-02T22:40:17.124464Z",
				"$lastUpdatedVersion": 11,
				"prodRate": {
					"$lastUpdated": "2024-06-02T22:40:17.124464Z",
					"$lastUpdatedVersion": 11
				}
			},
			"$version": 11
		},
		"reported": {
			"errorsCode": 1,
			"errorsList": "Emergency stop ",
			"prodRate": 30,
			"$metadata": {
				"$lastUpdated": "2024-06-03T23:24:14.6153246Z",
				"errorsCode": {
					"$lastUpdated": "2024-06-03T23:24:14.6153246Z"
				},
				"errorsList": {
					"$lastUpdated": "2024-06-03T23:24:14.6153246Z"
				},
				"prodRate": {
					"$lastUpdated": "2024-06-03T23:24:14.6153246Z"
				}
			},
			"$version": 916
		}
	},
	"capabilities": {
		"iotEdge": false
	}
}
```

To change the "production rate" of the device, you need

- Add a field called "prod_rate" and the required value from 0 to 100 in the "desired" field
- Click the "Save" button
- Click the "Reload" button

Now you can see the changed parameter in the "reported" field

![Screenshot_16](https://github.com/Belt0M/IotHub/assets/89530659/84a8323c-ab84-4fbf-81e6-ae32d2cb5171)

## 5. Performing Device Direct Methods

- Open the "Direct method" section
- Enter the name of one of the methods expected below in the "Method name" field
- Click the "Invoke method" button

### Available methods:

- **EmergencyStop**: Emergency stops the device
- **ResetErrorStatus**: Resets all device error statuses

After applying the method, you can observe the result either in device twin or in the devices simulator

![Screenshot_17](https://github.com/Belt0M/IotHub/assets/89530659/9d03050f-f0fd-4619-a467-fded1a1384a1)
![Screenshot_18](https://github.com/Belt0M/IotHub/assets/89530659/4c601b13-4741-4fec-b40b-4fc05af5f6c0)
![Screenshot_19](https://github.com/Belt0M/IotHub/assets/89530659/58a0f78c-0af1-47cf-a826-85cb424f55c2)

## 6. Performing Data Calculations

Data calculations are performed using Azure Stream Analytics Jobs. To start you need:

- Create "Storage account"
- Create 2 blob containers (for example: "temparature", "production _kpi ")
- Create "Stream Analytics Job"
- Go to the newly created job
- Under "Job topology" select "Inputs" and add your "IoTHub" as a data source for analysis
- Similarly, add recently created blob containers as places to save analysis results (in "Outputs" subsection)
- In the Query subsection, insert the code of the desired request, see below

![Screenshot_20](https://github.com/Belt0M/IotHub/assets/89530659/d21715b9-76f7-493b-9a2b-ea34871ffbdb)
![Screenshot_21](https://github.com/Belt0M/IotHub/assets/89530659/e5a5fbaa-36e2-474b-b950-9e108df15e63)
![Screenshot_22](https://github.com/Belt0M/IotHub/assets/89530659/cc26247a-6609-4a75-b65c-69abdd1ec67f)
![Screenshot_23](https://github.com/Belt0M/IotHub/assets/89530659/63f0f299-ea99-47bf-996f-f834256382c0)
![Screenshot_24](https://github.com/Belt0M/IotHub/assets/89530659/594fd4d2-29da-4deb-8731-285877db3e05)
![Screenshot_25](https://github.com/Belt0M/IotHub/assets/89530659/6d724f3a-857b-4cbb-bdc2-66500fc31c3a)

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

![Screenshot_26](https://github.com/Belt0M/IotHub/assets/89530659/4f163c34-2ede-44a0-8045-f41f646a69f5)





