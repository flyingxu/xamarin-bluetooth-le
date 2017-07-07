using Plugin.BLE.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using System.Threading;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Plugin.BLE.Extensions;

namespace Plugin.BLE.UWP
{
    public class Adapter : AdapterBase
    {
        private DeviceWatcher deviceWatcher;
        
        // The Bluetooth LE advertisement watcher class is used to control and customize Bluetooth LE scanning.
        private BluetoothLEAdvertisementWatcher watcher;

        private BluetoothAdapter _bluetoothAdapter;

        private ObservableCollection<DeviceInformation> ResultCollection = new ObservableCollection<DeviceInformation>();

        public override IList<IDevice> ConnectedDevices => ConnectedDeviceRegistry.Values.ToList();

        /// <summary>
        /// Used to store all connected devices
        /// key: Guid in string format.
        /// </summary>
        public Dictionary<string, IDevice> ConnectedDeviceRegistry { get; }

        public Adapter(BluetoothAdapter bluetoothAdapter)
        {
            _bluetoothAdapter = bluetoothAdapter;

            ConnectedDeviceRegistry = new Dictionary<string, IDevice>();
        }

        public override Task<IDevice> ConnectToKnownDeviceAsync(Guid deviceGuid, ConnectParameters connectParameters = default(ConnectParameters), CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public override List<IDevice> GetSystemConnectedOrPairedDevices(Guid[] services = null)
        {
            return new List<IDevice>();//todo
        }

        protected override async Task ConnectToDeviceNativeAsync(IDevice device, ConnectParameters connectParameters, CancellationToken cancellationToken)
        {
            var connectDevice = (Device) device;

            if (connectDevice != null)
            {
                var bluetoothDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(connectDevice.BluetoothAddress);

                if (bluetoothDevice != null)
                {
                    connectDevice.SetNativeDevice(bluetoothDevice);

                    ConnectedDeviceRegistry[device.Id.ToString()] = connectDevice;
                    HandleConnectedDevice(connectDevice);
                }
                else
                {
                    Trace.Message("Warning: Connect failed");
                }
            }
        }

        protected override void DisconnectDeviceNative(IDevice device)
        {
            throw new NotImplementedException();
        }

        protected override Task StartScanningForDevicesNativeAsync(Guid[] serviceUuids, bool allowDuplicatesKey, CancellationToken scanCancellationToken)
        {
            // Additional properties we would like about the device.
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            // BT_Code: Currently Bluetooth APIs don't provide a selector to get ALL devices that are both paired and non-paired.
            deviceWatcher =
                DeviceInformation.CreateWatcher(
                    "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")",
                    requestedProperties,
                    DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Start over with an empty collection.
            DiscoveredDevices.Clear();

            // Start the watcher.
            deviceWatcher.Start();

            //advertisement
            watcher = new BluetoothLEAdvertisementWatcher();
            watcher.Received += WatcherOnReceived;
            watcher.Stopped += WatcherOnStopped;
            watcher.Start();

            return Task.FromResult(true);
        }

        private void WatcherOnStopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void WatcherOnReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            if (sender == watcher)
            {
                if(args.AdvertisementType == BluetoothLEAdvertisementType.ConnectableUndirected)
                {
                    var localName = args.Advertisement.LocalName;

                    var device = new Device(this, localName, args.BluetoothAddress, args.Advertisement.DataSections, args.RawSignalStrengthInDBm);
                    HandleDiscoveredDevice(device);
                }
            }
        }

        protected override void StopScanNative()
        {
            if (deviceWatcher != null)
            {
                // Unregister the event handlers.
                deviceWatcher.Added -= DeviceWatcher_Added;
                deviceWatcher.Updated -= DeviceWatcher_Updated;
                deviceWatcher.Removed -= DeviceWatcher_Removed;
                deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
                deviceWatcher.Stopped -= DeviceWatcher_Stopped;

                // Stop the watcher.
                deviceWatcher.Stop();
                deviceWatcher = null;
            }
        }


        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            // Protect against race condition if the task runs after the app stopped the deviceWatcher.
            if (sender == deviceWatcher)
            {
                var properties = deviceInfo.Properties;

            }
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            // Protect against race condition if the task runs after the app stopped the deviceWatcher.
            if (sender == deviceWatcher)
            {
                //var discovereDevice = DiscoveredDevices.FirstOrDefault(device => ((Device)device).DeviceInformation.Id == deviceInfoUpdate.Id);

                //((Device) discovereDevice)?.DeviceInformation.Update(deviceInfoUpdate);
            }
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            // Protect against race condition if the task runs after the app stopped the deviceWatcher.
            if (sender == deviceWatcher)
            {
                //var discovereDevice = DiscoveredDevices.FirstOrDefault(device => ((Device)device).DeviceInformation.Id == deviceInfoUpdate.Id);

                //if(discovereDevice != null)
                //    HandleDisconnectedDevice(false,discovereDevice);
               // else
                {
                    //weird   
                }
            }
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object e)
        {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    Trace.Message($"{ResultCollection.Count} devices found. Enumeration completed.");
                }
        }

        private async void DeviceWatcher_Stopped(DeviceWatcher sender, object e)
        {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    Trace.Message($"No longer watching for devices.");
                        //sender.Status == DeviceWatcherStatus.Aborted ? NotifyType.ErrorMessage : NotifyType.StatusMessage);
                }
        }

        private DeviceInformation FindBluetoothLEDeviceDisplay(string id)
        {
            foreach (var bleDeviceDisplay in ResultCollection)
            {
                if (bleDeviceDisplay.Id == id)
                {
                    return bleDeviceDisplay;
                }
            }
            return null;
        }
    }
}
