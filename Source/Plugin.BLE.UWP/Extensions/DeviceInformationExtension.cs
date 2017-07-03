using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace Plugin.BLE.Extensions
{
    public static class DeviceInformationExtension 
    {
        //BluetoothLE#BluetoothLEa4:34:d9:3e:c3:c5-88:0f:10:a2:78:7e
        public static Guid ToGuid(this DeviceInformation deviceInformation)
        {
            var id = deviceInformation.Id;
            var deviceGuid = new byte[16];
            var macWithoutColons = id.Replace("BluetoothLE", String.Empty).Replace("#", String.Empty)
                .Replace(":", String.Empty).Replace("-", String.Empty);
            var macBytes = Enumerable.Range(0, macWithoutColons.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(macWithoutColons.Substring(x, 2), 16))
                .ToArray();
            macBytes.CopyTo(deviceGuid, 16 - 12);
            return new Guid(deviceGuid);
        }
    }
}
