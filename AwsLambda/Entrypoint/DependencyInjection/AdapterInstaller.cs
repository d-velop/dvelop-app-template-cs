using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Dvelop.Lambda.EntryPoint.DependencyInjection
{
    public class AdapterInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("Adapter")
                .Pick()
                .WithServiceDefaultInterfaces()
            );
        }
    }
}