using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Radios;
using Plugin.BLE.Abstractions.Contracts;

namespace Plugin.BLE.Extensions
{
    public static class BluetoothStateExtension
    {
        public static BluetoothState ToBluetoothState(this RadioState state)
        {
            switch (state)
            {
                case RadioState.Unknown:
                    return BluetoothState.Unknown;                    
                case RadioState.On:
                    return BluetoothState.On;
                case RadioState.Off:
                    return BluetoothState.Off;
                case RadioState.Disabled:
                    //The radio is powered off and disabled by the device firmware or a hardware switch on the device.
                    return BluetoothState.Off;
                default:
                    return BluetoothState.Unknown;
            }
        }
    }
}
