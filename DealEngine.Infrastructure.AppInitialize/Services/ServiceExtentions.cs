using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using TechCertain.Services.Impl;

namespace DealEngine.Infrastructure.AppInitialize.Services
{
    public static class RespositoriesExtentions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {

            var repositoryAssembly = typeof(EmailService).Assembly;
            var registrations =
                from type in repositoryAssembly.GetExportedTypes()
                where type.Namespace == "TechCertain.Services.Impl"
                where type.GetInterfaces().Any()
                select new { Service = type.GetInterfaces().Single(), Implementation = type };

            foreach (var reg in registrations)
            {
                services.AddTransient(reg.Service, reg.Implementation);
            }

            return services;
        }
    }

    
}
