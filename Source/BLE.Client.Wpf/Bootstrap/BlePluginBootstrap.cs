using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform.Plugins;

namespace BLE.Client.Wpf.Bootstrap
{
    public class BlePluginBootstrap
        :MvxLoaderPluginBootstrapAction<MvvmCross.Plugins.BLE.PluginLoader, MvvmCross.Plugins.BLE.Wpf.Plugin>
    {
    }
}
