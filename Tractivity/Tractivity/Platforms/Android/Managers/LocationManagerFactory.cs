using Android.Content;
using Tractivity.AppServices;
using Tractivity.Contract.Enums;

namespace Tractivity.Managers
{
    public class LocationManagerFactory : ILocationManagerFactory
    {
        public void Initialize(ServiceType serviceType)
        {
            if (serviceType.Equals(ServiceType.Walking))
            {
                this.StartService<WalkingService>();
            }
        }

        public void Stop(ServiceType serviceType)
        {
            if (serviceType.Equals(ServiceType.Walking))
            {
                this.StopService<WalkingService>();
            }
        }

        private void StartService<TService>()
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

        private void StopService<TService>()
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(TService));
            Android.App.Application.Context.StopService(intent);
        }
    }
}