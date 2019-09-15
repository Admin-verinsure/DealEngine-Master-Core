using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DealEngine.Infrastructure.Identity.Subservices
{
    public class DealEngineServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return serviceType;
        }
    }
}

