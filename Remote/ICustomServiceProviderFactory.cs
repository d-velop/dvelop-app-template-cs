using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dvelop.Remote
{
    public interface ICustomServiceProviderFactory
    {
        IServiceProvider CreateServiceProvider(IServiceCollection services);
    }
}