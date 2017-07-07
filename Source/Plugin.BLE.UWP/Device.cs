using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Extensions;

namespace Plugin.BLE.UWP
{
    public class Device : DeviceBase
    {     
        private BluetoothLEDevice _nativeDevice;        //valid after connection

        public Device(IAdapter adapter, string name, ulong bluetoothAddress, IList<BluetoothLEAdvertisementDataSection> dataSections, int rssi) 
            : base(adapter)
        {
            Name = name;

            BluetoothAddress = bluetoothAddress;
            Id = new Guid(0,0,0,0,0,
                (byte)(bluetoothAddress >> 40),
                (byte)(bluetoothAddress >> 32),
                (byte)(bluetoothAddress >> 24),
                (byte)(bluetoothAddress >> 16),
                (byte)(bluetoothAddress >> 8),
                (byte)bluetoothAddress);

            AdvertisementRecords = new List<AdvertisementRecord>();
            foreach (var dataSection in dataSections)
            {
                AdvertisementRecords.Add(new AdvertisementRecord((AdvertisementRecordType)dataSection.DataType, dataSection.Data.ToArray()));
            }

            Rssi = rssi;
        }

        public ulong BluetoothAddress { get; }

        public override object NativeDevice => _nativeDevice;

        public void SetNativeDevice(BluetoothLEDevice leDevice)
        {
            _nativeDevice = leDevice;
        }

        public override Task<bool> UpdateRssiAsync()
        {
            return Task.FromResult(false);
        }

        protected override DeviceState GetState()
        {
            if(_nativeDevice == null)
                return DeviceState.Disconnected;

            switch (_nativeDevice.ConnectionStatus)
            {
                case BluetoothConnectionStatus.Disconnected:
                    return DeviceState.Disconnected;
                case BluetoothConnectionStatus.Connected:
                    return DeviceState.Connected;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override async Task<IEnumerable<IService>> GetServicesNativeAsync()
        {
            var gattServiceResult = await _nativeDevice.GetGattServicesAsync();

            return gattServiceResult.Services.Select(service => new Service(this, service)).Cast<IService>().ToList();
        }

        protected override async Task<int> RequestMtuNativeAsync(int requestValue)
        {
            return -1;
        }
    }
}
