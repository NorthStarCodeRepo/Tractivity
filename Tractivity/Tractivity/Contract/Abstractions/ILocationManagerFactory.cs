using Tractivity.Contract.Enums;

namespace Tractivity.Managers
{
    public interface ILocationManagerFactory
    {
        void Initialize(ServiceType serviceType);

        void Stop(ServiceType serviceType);
    }
}