using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

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

            Id = ParseDeviceId(deviceInformation.Id);
            Name = deviceInformation.Name;
        }

        public override object NativeDevice => _nativeDevice;

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

        //BluetoothLE#BluetoothLEa4:34:d9:3e:c3:c5-88:0f:10:a2:78:7e
        private Guid ParseDeviceId(string id)
        {
            var deviceGuid = new byte[16];
            var macWithoutColons = id.Replace("BluetoothLE", String.Empty).Replace("#", String.Empty)
                .Replace(":", String.Empty).Replace("-", String.Empty);
            var macBytes = Enumerable.Range(0, macWithoutColons.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(macWithoutColons.Substring(x, 2), 16))
                .ToArray();
            macBytes.CopyTo(deviceGuid, 16-12);
            return new Guid(deviceGuid);
        }
    }
}
