using Tractivity.Common.Environment;

namespace Tractivity.AppServices
{
    public class LocationService : ILocationService
    {
        private readonly EnvironmentManager _environmentManager;

        public LocationService(EnvironmentManager environmentManager)
        {
            this._environmentManager = environmentManager;
        }
    }
}