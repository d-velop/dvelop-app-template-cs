using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.AspNetCore.Mvc;

namespace Dvelop.Selfhosted.HostApplication.DependencyInjection
{
    public class ControllerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyNamed("Remote")
                .Pick()
                .WithServiceDefaultInterfaces()
            );
            container.Register(Classes.FromAssemblyNamed("Remote")
                .BasedOn<Controller>()
                .LifestyleTransient()
            );
        }
    }
}