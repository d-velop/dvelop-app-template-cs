using System;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Dvelop.Domain.Repositories;
using Dvelop.Domain.Vacation;
using Dvelop.Plugins.DynamoDbFake;
using Dvelop.Remote;
using Microsoft.Extensions.DependencyInjection;

namespace Dvelop.Lambda.EntryPoint.DependencyInjection
{
    public class AwsServiceProviderFactory: ICustomServiceProviderFactory
    {
        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            var windsorContainer = new WindsorContainer();

            windsorContainer.Install(new DomainInstaller());
            windsorContainer.Install(new ControllerInstaller());
            windsorContainer.Install(new AdapterInstaller());
            services.AddSingleton<IVacationRepository, AwsVacationRepository>();
            services.AddSingleton<IBusinessValueRepository, AwsBusinessValueRepository>();
            return WindsorRegistrationHelper.CreateServiceProvider(windsorContainer, services);
        }
    }
}