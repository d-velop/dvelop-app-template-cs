using System;
using Dvelop.Domain.ExampleBusinessLogic;
using Dvelop.Domain.Repositories;
using Dvelop.Domain.Vacation;
using Dvelop.Domain.VersionService;
using Dvelop.Plugins.InMemoryDb;
using Dvelop.Plugins.WebApi;
using Dvelop.Remote;
using Microsoft.Extensions.DependencyInjection;

namespace Dvelop.Selfhosted.HostApplication.DependencyInjection
{
    public class SelfHostedServiceProviderFactory: ICustomServiceProviderFactory
    {

        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            // Serviceinstaller for Plugins using the WebApi (Values provided by the Request)
            services.AddSingleton<ITenantRepository, TenantRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();

            // Serviceinstaller for Domain Logic
            services.AddSingleton<IVersionService, VersionService>();
            services.AddSingleton<IVacationService, VacationService>();
            services.AddSingleton<IExampleBusinessLogicService, ExampleBusinessLogicService>();

            //Serviceinstaller for InMemoryDatabase
            services.AddSingleton<IVacationRepository, InMemoryVacationRepository>();
            services.AddSingleton<IBusinessValueRepository, InMemoryBusinessValueRepository>();
            return services.BuildServiceProvider();
        }
    }
}