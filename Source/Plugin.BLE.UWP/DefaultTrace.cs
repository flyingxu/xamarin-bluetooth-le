using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions;

namespace Plugin.BLE.UWP
{
    static class DefaultTrace
    {
        static DefaultTrace()
        {
            //Trace.TraceImplementation = Debug.WriteLine;
        }
    }
}
