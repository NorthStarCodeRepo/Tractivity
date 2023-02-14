using Android.App;
using Android.Content;
using Tractivity.Contract.Enums;
using Tractivity.Platforms.Android.AppServices;

namespace Tractivity.Managers
{
    public partial class LocationManagerFactory : ILocationManagerFactory
    {
        public void Initialize(ServiceType serviceType)
        {
            if (serviceType.Equals(ServiceType.Walking))
            {
                this.StartService<LocationService>();
            }
        }

        public void Stop(ServiceType serviceType)
        {
            if (serviceType.Equals(ServiceType.Walking))
            {
                this.StopService<LocationService>();
            }
        }

        private void StartService<TService>() where TService : Service
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(TService));

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                Android.App.Application.Context.StartForegroundService(intent);
            }
            else
            {
                Android.App.Application.Context.StartService(intent);
            }
        }

        private void StopService<TService>() where TService : Service
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(TService));
            Android.App.Application.Context.StopService(intent);
        }
    }
}