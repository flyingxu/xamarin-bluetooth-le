using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace Plugin.BLE.UWP
{
    public class Descriptor : DescriptorBase
    {
        private GattDescriptor _nativeDescriptor;

        public Descriptor(GattDescriptor descriptor, ICharacteristic characteristic) : base(characteristic)
        {
            _nativeDescriptor = descriptor;
        }

        public override Guid Id { get; }
        public override byte[] Value { get; }
        protected override async Task<byte[]> ReadNativeAsync()
        {
            var gattReadResult = await _nativeDescriptor.ReadValueAsync();
            if (gattReadResult.Status == GattCommunicationStatus.Success)
                return gattReadResult.Value.ToArray();

            throw new TaskCanceledException();
        }

        protected override async Task WriteNativeAsync(byte[] data)
        {
            await _nativeDescriptor.WriteValueWithResultAsync(data.AsBuffer());
        }
    }
}
