using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IApplicationLoggingService
    {
        Task LogInformation(ILogger logger, Exception ex, User user, HttpContext httpContext);
        Task LogWarning(ILogger logger, Exception ex, User user, HttpContext httpContext);
    }
}
