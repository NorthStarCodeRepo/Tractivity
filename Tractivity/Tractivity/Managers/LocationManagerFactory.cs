using Tractivity.Contract.Enums;

namespace Tractivity.Managers
{
    public partial class LocationManagerFactory
    {
        public partial void Initialize(ServiceType serviceType);

        public partial void Stop(ServiceType serviceType);
    }
}