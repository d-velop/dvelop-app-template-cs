using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dvelop.Remote
{
    public class EnvironmentAwareServiceProviderFactory: IServiceProviderFactory<IServiceCollection>
    {
        private readonly ICustomServiceProviderFactory _customServiceProviderFactory;

        public EnvironmentAwareServiceProviderFactory(ICustomServiceProviderFactory customServiceProviderFactory)
        {
            _customServiceProviderFactory = customServiceProviderFactory;
        }

        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            return _customServiceProviderFactory.CreateServiceProvider(services);
        }
    }
}