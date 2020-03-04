using ElmahCore;
using ElmahCore.Postgresql;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;


namespace DealEngine.Services.Impl
{
    public class ApplicationLoggingService : IApplicationLoggingService
    {
        IAppSettingService _appSettingService;
        public ApplicationLoggingService(IAppSettingService appSettingService)
        {
            _appSettingService = appSettingService;
        }


        public async Task LogInformation(ILogger logger, Exception ex, User user, HttpContext httpContext)
        {            
            logger.LogInformation(ex.Message, DateTime.Now, user.UserName, ex.StackTrace);
            httpContext.RiseError(ex);
            ElmahExtensions.RiseError(ex);            
        }

        public async Task LogWarning(ILogger logger, Exception ex, User user, HttpContext httpContext)
        {
            var userName = "";
            if(user == null)
            {
                userName = "No User";
            }
            else
            {
                userName = user.UserName;
            }

            PgsqlErrorLog pgsqlErrorLog = new PgsqlErrorLog(_appSettingService.GetConnectionString);
            Error error = new Error(ex);
            error.Time = DateTime.Now;
            pgsqlErrorLog.Log(error);
            logger.LogWarning(ex.Message, DateTime.Now, userName, ex.StackTrace);
            httpContext.RiseError(ex);
        }
    }
}

