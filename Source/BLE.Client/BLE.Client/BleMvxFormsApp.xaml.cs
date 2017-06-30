using MvvmCross.Platform;
using MvvmCross.Forms.Core;

namespace BLE.Client
{
    public partial class BleMvxFormsApp : MvxFormsApplication
    {
        public BleMvxFormsApp()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            base.OnStart();
            Mvx.Trace("App Start");
        }

        protected override void OnResume()
        {
            base.OnResume();
            Mvx.Trace("App Resume");
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            Mvx.Trace("App Sleep");
        }
    }
}
