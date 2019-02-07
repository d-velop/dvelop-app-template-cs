using System;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Dvelop.Domain.Repositories;
using Dvelop.Domain.Vacation;
using Dvelop.Plugins.InMemoryDb;
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
            
            services.AddSingleton<IVacationRepository, SelfHostedVacationRepository>();
            services.AddSingleton<IBusinessValueRepository, SelfHostedBusinessValueRepository>();
            return WindsorRegistrationHelper.CreateServiceProvider(windsorContainer, services);
        }
    }
}