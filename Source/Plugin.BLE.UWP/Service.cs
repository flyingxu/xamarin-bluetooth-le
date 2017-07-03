using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace Plugin.BLE.UWP
{
    public class Service : ServiceBase
    {
        private readonly GattDeviceService _nativeService;

        public Service(IDevice device, GattDeviceService service) : base(device)
        {
            _nativeService = service;
        }

        public override Guid Id => _nativeService.Uuid;

        public override bool IsPrimary => false;

        protected override async Task<IList<ICharacteristic>> GetCharacteristicsNativeAsync()
        {
            var gattCharacteristicsResult = await _nativeService.GetCharacteristicsAsync();

            return gattCharacteristicsResult.Characteristics.Select(characteristic => new Characteristic(this, characteristic))
                .Cast<ICharacteristic>().ToList();
        }
    }
}
