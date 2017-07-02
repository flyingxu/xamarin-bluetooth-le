using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Radios;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Extensions;
using Plugin.BLE.UWP;

namespace Plugin.BLE
{
    internal class BleImplementation : BleImplementationBase
    {
        private BluetoothAdapter _bluetoothAdapter;
        private Radio _radio;

        protected override IAdapter CreateNativeAdapter()
        {
            if(_bluetoothAdapter != null)
                return new Adapter(_bluetoothAdapter);
            else
            {
                return null;
            }
        }

        protected override BluetoothState GetInitialStateNative()
        {
            return BluetoothState.Unknown;
        }

        protected override void InitializeNative()
        {
            BluetoothAdapter.GetDefaultAsync().AsTask().ContinueWith(OnGetDefaultAdapter);

        }

        private void OnGetDefaultAdapter(Task<BluetoothAdapter> taskGetAdapter)
        {
            _bluetoothAdapter = taskGetAdapter.Result;

            //if the system supports bluetooth LE
            if (_bluetoothAdapter.IsLowEnergySupported)
            {
                _bluetoothAdapter.GetRadioAsync().AsTask().ContinueWith(OnRadio);
            }
            else
            {
                State = BluetoothState.Unavailable;
            }
        }

        private void OnRadio(Task<Radio> taskRadio)
        {
            _radio = taskRadio.Result;

            //for future state change
            _radio.StateChanged += (sender, args) =>
            {
                State = _radio.State.ToBluetoothState();
            };

            State = _radio.State.ToBluetoothState();
        }
    }
}
