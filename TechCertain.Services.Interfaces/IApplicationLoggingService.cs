using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IApplicationLoggingService
    {
        Task LogInformation(ILogger logger, Exception ex, User user, HttpContext httpContext);
        Task LogWarning(ILogger logger, Exception ex, User user, HttpContext httpContext);
    }
}
