using Shiny.BluetoothLE;
using Shiny;
using System.Collections.Concurrent;
using System.Text;
using System.Reactive.Linq;

namespace hio_dotnet.PhoneDrivers.BLE
{
    public class ChesterBLEService
    {
        private readonly IBleManager bleManager;
        public ConcurrentDictionary<string, IPeripheral> PeripherialsDict = new ConcurrentDictionary<string, IPeripheral>();
        public event EventHandler<EventArgs> PeripherialsDictChanged;
        public event EventHandler<EventArgs> Connected;
        public event EventHandler<EventArgs> DeviceDetialsLoaded;
        public event EventHandler<string> NewConsoleLineOutputReceived;
        public ConnectedDevice ConnectedDevice { get; set; } = new ConnectedDevice();

        public IPeripheral? ConnectedPeripheral { get; set; }

        private bool isSubscribed = false;
        private string vendorName = "6e7e8548-7481-4822-a238-7607ad2f667d";
        private string productName = "b3249503-1cc7-4200-a9c1-3d8ab2c3235c";
        private string hardwareVariant = "adc8c4d2-ef7c-425a-af5b-1ce9c94dc51c";
        private string hardwareRevision = "12e740c2-5bf4-48f6-a781-8b064b21622c";
        private string firmwareName = "0e5f5dfe-afef-4725-b19f-92e47801c721";
        private string firmwareVersion = "56917801-e1a0-476a-ab86-07e61076d6d9";
        private string serialNumber = "0cd70392-53b9-4518-b155-f5f7474ec28e";
        private string claimToken = "a24a59ae-98d5-4c20-aa0e-0dc0fde88b96";
        private string bluetoothAddress = "e5c97616-10fe-4f04-988a-8f904a81f974";
        private string bluetoothPassKey = "18c18da9-ddbc-49be-a241-1144aab2b5fa";

        private string uartservices = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
        private string uartTxChar = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E";
        private string uartRxChar = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";

        public ChesterBLEService(IBleManager bleManager)
        {
            this.bleManager = bleManager;
        }

        public async Task ScanForDevices()
        {
            var access = await this.bleManager.RequestAccess();
            if (access == AccessState.Available)
            {
                var scan = this.bleManager.Scan().Subscribe(scanResult =>
                {
                    var device = scanResult.Peripheral;
                    if (device != null)
                    {
                        //Console.WriteLine($"Found device: {device.Name}");
                        if (device.Name != null)
                        {
                            if (!PeripherialsDict.ContainsKey(device.Name))
                            {
                                PeripherialsDict.TryAdd(device.Name, device);
                                PeripherialsDictChanged?.Invoke(this, new EventArgs());
                            }
                        }
                    }
                });
            }
        }

        public async Task ConnectToDevice(IPeripheral peripheral)
        {
            await peripheral.ConnectAsync();

            peripheral.WhenConnected().Subscribe(x =>
            {
                Console.WriteLine("Connected." + x.Name);
                ConnectedPeripheral = peripheral;
                Connected?.Invoke(this, new EventArgs());
            });
            peripheral.WhenDisconnected().Subscribe(x => { Console.WriteLine("Disconnected." + x.Name); });
            peripheral.WhenStatusChanged().Subscribe(connectionState => { Console.WriteLine("Status Changed." + connectionState.ToString()); });

            // Discover services and characteristics
        }

        public async Task RegisterToConsoleOutputs(IPeripheral peripheral)
        {
            var rxchar = await peripheral.GetCharacteristicAsync(uartservices, uartRxChar);
            if (rxchar != null)
            {
                if (!rxchar.CanNotify())
                {
                    Console.WriteLine("RX charakteristika nepodporuje notifikace.");
                    return;
                }

                try
                {
                    var receivedDataBuilder = new StringBuilder();

                    if (!isSubscribed)
                    {
                        var rxResults = peripheral.NotifyCharacteristic(rxchar);
                        isSubscribed = true;

                        var rxSubscription = rxResults.Subscribe(result =>
                        {
                            if (result.Data != null)
                            {
                                var receivedChunk = Encoding.UTF8.GetString(result.Data);
                                //Console.WriteLine("Data Received Chunk: " + receivedChunk);

                                receivedDataBuilder.Append(receivedChunk);

                                if (receivedChunk.Contains("s:~$ ")) // Whole message end
                                {
                                    var completeData = receivedDataBuilder.ToString();
                                    Console.WriteLine("Complete Data Received: " + completeData);
                                    // push each line in completeData through event NewConsoleLineOutputReceived
                                    foreach (var line in completeData.Split('\n'))
                                    {
                                        NewConsoleLineOutputReceived?.Invoke(this, line);
                                    }

                                    receivedDataBuilder.Clear();
                                }
                            }
                        });
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during subscribing to notifications from UART: " + ex.Message);
                }
            }
        }

        public async Task SendCommand(IPeripheral peripheral, string command)
        {
            var txchar = await peripheral.GetCharacteristicAsync(uartservices, uartTxChar);
            if (txchar != null)
            {
                if (!isSubscribed)
                    await RegisterToConsoleOutputs(peripheral);

                try
                {
                    var message = Encoding.UTF8.GetBytes(command);
                    var txResult = await peripheral.WriteCharacteristicAsync(txchar, message);
                    if (txResult != null)
                    {
                        Console.WriteLine("Data sent.");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during communication: " + ex.Message);
                }
            }
        }

        public async Task GetChesterDescriptionData(IPeripheral peripheral)
        {
            var chars = await peripheral.GetCharacteristicsAsync(uartservices);
            foreach (var character in chars)
            {
                Console.WriteLine("Char1: " + character.Uuid);
            }
            var chars1 = await peripheral.GetCharacteristicsAsync("8f4afdba-ae12-465b-a871-13bd7182017c");
            foreach (var character in chars1)
            {
                Console.WriteLine("Char1: " + character);
                var result = await peripheral.ReadCharacteristicAsync(character);
                if (result != null && result.Data != null)
                {
                    Console.WriteLine("Characteristic: " + Encoding.UTF8.GetString(result.Data));
                    if (character.Uuid == vendorName)
                        ConnectedDevice.VendorName = Encoding.UTF8.GetString(result.Data);
                    else if (character.Uuid == productName)
                        ConnectedDevice.ProductName = Encoding.UTF8.GetString(result.Data);
                    else if (character.Uuid == hardwareVariant)
                        ConnectedDevice.HardwareVariant = Encoding.UTF8.GetString(result.Data);
                    else if (character.Uuid == hardwareRevision)
                        ConnectedDevice.HardwareRevision = Encoding.UTF8.GetString(result.Data);
                    else if (character.Uuid == firmwareName)
                        ConnectedDevice.FirmwareName = Encoding.UTF8.GetString(result.Data);
                    else if (character.Uuid == firmwareVersion)
                        ConnectedDevice.FirmwareVersion = Encoding.UTF8.GetString(result.Data);
                    else if (character.Uuid == serialNumber)
                        ConnectedDevice.SerialNumber = Encoding.UTF8.GetString(result.Data);
                    else if (character.Uuid == bluetoothAddress)
                        ConnectedDevice.BluetoothAddress = Encoding.UTF8.GetString(result.Data);
                    else if (character.Uuid == bluetoothPassKey)
                        ConnectedDevice.BluetoothKey = Encoding.UTF8.GetString(result.Data);
                    else if (character.Uuid == claimToken)
                        ConnectedDevice.ClaimToken = Encoding.UTF8.GetString(result.Data);

                    DeviceDetialsLoaded?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
