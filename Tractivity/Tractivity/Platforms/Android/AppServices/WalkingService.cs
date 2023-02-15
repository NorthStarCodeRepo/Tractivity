using Android.App;

namespace Tractivity.AppServices
{
    [Service]
    public class WalkingService : LocationService
    {
        public WalkingService()
        {
            this.Initialize(2000, 4);
        }
    }
}