using System;
using Dvelop.Adapter;
using Dvelop.Domain.ExampleBusinessLogic;
using Dvelop.Domain.Repositories;
using Dvelop.Domain.Vacation;
using Dvelop.Domain.VersionService;
using Dvelop.Plugins.DynamoDbFake;
using Dvelop.Plugins.WebApi;
using Dvelop.Remote;
using Microsoft.Extensions.DependencyInjection;

namespace Dvelop.Lambda.EntryPoint.DependencyInjection
{
    public class AwsServiceProviderFactory: ICustomServiceProviderFactory
    {
        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {

            services.AddSingleton<ITenantRepository, TenantRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();

            services.AddSingleton<IVersionService, VersionService>();
            services.AddSingleton<IVacationService, VacationService>();
            services.AddSingleton<IExampleBusinessLogicService, ExampleBusinessLogicService>();
            
            services.AddSingleton<IVacationRepository, DynamoDbVacationRepository>();
            services.AddSingleton<IBusinessValueRepository, DynamoDbBusinessValueRepository>();
            return services.BuildServiceProvider();
        }
    }
}