using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Dvelop.Selfhosted.HostApplication.DependencyInjection
{
    public class DomainInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("Domain")
                .Pick()
                .WithService
                .DefaultInterfaces()
            );
        }
    }
}
