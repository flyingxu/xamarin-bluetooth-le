using Acr.UserDialogs;
using BLE.Client.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Plugin.Permissions;
using Plugin.Settings;

namespace BLE.Client
{
    public class BleMvxApplication : MvxApplication
    {
        public override void Initialize()
        {
            Mvx.RegisterSingleton<IUserDialogs>(() => UserDialogs.Instance);
            Mvx.RegisterSingleton(() => CrossSettings.Current);
            Mvx.RegisterSingleton(() => CrossPermissions.Current);

            RegisterAppStart<DeviceListViewModel>();
        }
    }
}
