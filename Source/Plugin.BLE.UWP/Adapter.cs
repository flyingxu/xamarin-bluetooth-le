using Plugin.BLE.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using System.Threading;
using Windows.Devices.Enumeration;
using Windows.UI.Core;

namespace Plugin.BLE.UWP
{
    public class Adapter : AdapterBase
    {
        private DeviceWatcher deviceWatcher;

        private ObservableCollection<DeviceInformation> ResultCollection = new ObservableCollection<DeviceInformation>();

        public override IList<IDevice> ConnectedDevices => throw new NotImplementedException();

        public override Task<IDevice> ConnectToKnownDeviceAsync(Guid deviceGuid, ConnectParameters connectParameters = default(ConnectParameters), CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public override List<IDevice> GetSystemConnectedOrPairedDevices(Guid[] services = null)
        {
            throw new NotImplementedException();
        }

        protected override Task ConnectToDeviceNativeAsync(IDevice device, ConnectParameters connectParameters, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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

            return Task.FromResult(true);
        }

        protected override void StopScanNative()
        {
            throw new NotImplementedException();
        }


        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    // Make sure device name isn't blank or already present in the list.
                    if (deviceInfo.Name != string.Empty && FindBluetoothLEDeviceDisplay(deviceInfo.Id) == null)
                    {
                        //ResultCollection.Add(new BluetoothLEDeviceDisplay(deviceInfo));
                    }
                }
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    var bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                    if (bleDeviceDisplay != null)
                    {
                        bleDeviceDisplay.Update(deviceInfoUpdate);
                    }
                }
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    // Find the corresponding DeviceInformation in the collection and remove it.
                    var bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                    if (bleDeviceDisplay != null)
                    {
                        ResultCollection.Remove(bleDeviceDisplay);
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
