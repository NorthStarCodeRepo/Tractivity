using Tractivity.Platforms.Android.Services;

namespace Tractivity.Managers
{
    public partial class LocationManager
    {
        public partial void Initialize()
        {
            Android.Content.Intent intent = new Android.Content.Intent(Android.App.Application.Context, typeof(LocationService));
            Android.App.Application.Context.StartForegroundService(intent);
        }

        public partial void Stop()
        {
            Android.Content.Intent intent = new Android.Content.Intent(Android.App.Application.Context, typeof(LocationService));
            Android.App.Application.Context.StopService(intent);
        }
    }
}