using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Dvelop.Selfhosted.HostApplication.DependencyInjection
{
    public class AdapterInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("SelfHosted.Adapter")
                .Pick()
                .WithServiceDefaultInterfaces()
            );
            container.Register(Classes.FromAssemblyNamed("Adapter")
                .Pick()
                .WithServiceDefaultInterfaces()
            );
        }
    }
}