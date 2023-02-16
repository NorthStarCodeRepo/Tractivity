using Tractivity.Common.Environment;

namespace Tractivity.AppServices
{
    public class WalkingService : LocationService, IWalkingService
    {
        public WalkingService(EnvironmentManager environmentManager)
            : base(environmentManager)
        {
        }
    }
}