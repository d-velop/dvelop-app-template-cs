using System;
using Dvelop.Domain.Repositories;
using Dvelop.Domain.Vacation;
using Dvelop.Domain.VersionService;
using Dvelop.Plugins.DynamoDbFake;
using Dvelop.Plugins.WebApi;
using Dvelop.Remote;
using Microsoft.Extensions.DependencyInjection;

namespace Dvelop.Lambda.EntryPoint.DependencyInjection
{
    public class CustomServiceProviderFactory: ICustomServiceProviderFactory
    {
        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {

            services.AddSingleton<ITenantRepository, TenantRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();

            services.AddSingleton<IVersionService, VersionService>();
            services.AddSingleton<IVacationService, VacationService>();
            
            services.AddSingleton<IVacationRepository, DynamoDbVacationRepository>();
            return services.BuildServiceProvider();
        }
    }
}