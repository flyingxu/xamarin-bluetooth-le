using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace Plugin.BLE.UWP
{
    public class Characteristic : CharacteristicBase
    {
        private GattCharacteristic _nativeGattCharacteristic;
        public Characteristic(IService service, GattCharacteristic characteristic) : base(service)
        {
            _nativeGattCharacteristic = characteristic;
        }

        public override event EventHandler<CharacteristicUpdatedEventArgs> ValueUpdated;
        public override Guid Id => _nativeGattCharacteristic.Uuid;
        public override string Uuid => _nativeGattCharacteristic.Uuid.ToString();

        public override byte[] Value { get; }

        public override CharacteristicPropertyType Properties => CharacteristicPropertyType.Indicate;
        //    (CharacteristicPropertyType) _nativeGattCharacteristic.CharacteristicProperties;

        protected override async Task<IList<IDescriptor>> GetDescriptorsNativeAsync()
        {
            var gattDescriptorResult = await _nativeGattCharacteristic.GetDescriptorsAsync();

            return gattDescriptorResult.Descriptors.Select(descriptor => new Descriptor(descriptor, this)).Cast<IDescriptor>()
                .ToList();
        }

        protected override async Task<byte[]> ReadNativeAsync()
        {
            var gattReadResult = await _nativeGattCharacteristic.ReadValueAsync();

            if (gattReadResult.Status == GattCommunicationStatus.Success)
            {
                return gattReadResult.Value.ToArray();
            }
            
            throw new TaskCanceledException();
        }

        protected override async Task<bool> WriteNativeAsync(byte[] data, CharacteristicWriteType writeType)
        {
            var gattCommunicationStatus = await _nativeGattCharacteristic.WriteValueAsync(data.AsBuffer());

            return gattCommunicationStatus == GattCommunicationStatus.Success;
        }

        protected override Task StartUpdatesNativeAsync()
        {
            return Task.FromResult(true);
        }

        protected override Task StopUpdatesNativeAsync()
        {
            return Task.FromResult(true);
        }
    }
}
