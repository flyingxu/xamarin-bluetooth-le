using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using Trace = Plugin.BLE.Abstractions.Trace;

namespace Plugin.BLE.UWP
{
    public class Characteristic : CharacteristicBase
    {
        private readonly GattCharacteristic _nativeGattCharacteristic;

        public Characteristic(IService service, GattCharacteristic characteristic) : base(service)
        {
            _nativeGattCharacteristic = characteristic;
        }

        public override event EventHandler<CharacteristicUpdatedEventArgs> ValueUpdated;
        public override Guid Id => _nativeGattCharacteristic.Uuid;
        public override string Uuid => _nativeGattCharacteristic.Uuid.ToString();

        public override byte[] Value { get; }

        public override CharacteristicPropertyType Properties => 
            (CharacteristicPropertyType) _nativeGattCharacteristic.CharacteristicProperties;

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

        protected override async Task StartUpdatesNativeAsync()
        {
            _nativeGattCharacteristic.ValueChanged += OnValueChanged;

            var gattcommunicationStatus = await _nativeGattCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                GattClientCharacteristicConfigurationDescriptorValue.Notify);

            if(gattcommunicationStatus != GattCommunicationStatus.Success)
                throw new CharacteristicReadException("Gatt SetCharacteristicNotification FAILED.");

            Trace.Message("StartUpdatesNativeAsync, successful!");
        }

        private void OnValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            if (sender == _nativeGattCharacteristic)
            {
                byte[] data;
                CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out data);

                Debug.WriteLine(BitConverter.ToString(data));

                ValueUpdated?.Invoke(this, new CharacteristicUpdatedEventArgs(this));
            }

            Trace.Message("StartUpdatesNativeAsync, successful!");
        }

        protected override Task StopUpdatesNativeAsync()
        {
            return Task.FromResult(true);
        }
    }
}
