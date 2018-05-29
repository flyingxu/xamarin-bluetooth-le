using System;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace PluginNugetTest
{
    public class CompilerPleaseCheck
    {
        public void CheckMyAdapter(IAdapter adapter)
        {
            adapter.StartScanningForDevicesAsync();
        }

        public void CheckMyDevice(IDevice device)
        {
            device.UpdateRssiAsync();
        }

        public async Task CheckMyCharacteristic(ICharacteristic characteristic)
        {
            await characteristic.StartUpdatesAsync();
        }

        public void CheckMyService(IService service)
        {
            service.GetCharacteristicAsync(Guid.Empty);
        }
    }
}
