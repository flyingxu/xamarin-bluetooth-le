using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace Plugin.BLE
{
    internal class BleImplementation : BleImplementationBase
    {
        protected override IAdapter CreateNativeAdapter()
        {
            throw new NotImplementedException();
        }

        protected override BluetoothState GetInitialStateNative()
        {
            return BluetoothState.Unavailable;
        }

        protected override void InitializeNative()
        {
            //if the system supports bluetooth LE
        }
    }
}
