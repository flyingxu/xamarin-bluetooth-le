using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Extensions;

namespace Plugin.BLE.UWP
{
    public class Device : DeviceBase
    {
        private BluetoothLEDevice _nativeDevice;        //valid after connection

        public DeviceInformation DeviceInformation;   //valid before connection

        public Device(IAdapter adapter, DeviceInformation deviceInformation) 
            : base(adapter)
        {
            DeviceInformation = deviceInformation;

            Id = deviceInformation.ToGuid();
            Name = deviceInformation.Name;
        }

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
            return new List<IService>();
        }

        protected override async Task<int> RequestMtuNativeAsync(int requestValue)
        {
            return -1;
        }
    }
}
