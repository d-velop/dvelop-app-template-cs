using System;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Dvelop.Remote;
using Microsoft.Extensions.DependencyInjection;

namespace Dvelop.Selfhosted.HostApplication.DependencyInjection
{
    public class SelfHostedServiceProviderFactory: ICustomServiceProviderFactory
    {
        

        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            var windsorContainer = new WindsorContainer();

            windsorContainer.Install(new DomainInstaller());
            windsorContainer.Install(new ControllerInstaller());
            windsorContainer.Install(new AdapterInstaller());
            
            return WindsorRegistrationHelper.CreateServiceProvider(windsorContainer, services);
        }
    }
}