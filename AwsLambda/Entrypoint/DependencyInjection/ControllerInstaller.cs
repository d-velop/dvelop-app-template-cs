using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.AspNetCore.Mvc;

namespace Dvelop.Lambda.EntryPoint.DependencyInjection
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