using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace techcertain2015rebuildcore.Middleware.Authentication
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _policyName;
        public AuthorizationMiddleware(RequestDelegate next, string policyName)
        {
            _next = next;
            _policyName = policyName;
        }

        public async Task Invoke(HttpContext httpContext, IAuthorizationService authorizationService)
        {
            AuthorizationResult authorizationResult =
                await authorizationService.AuthorizeAsync(httpContext.User, null, _policyName);

            if (!authorizationResult.Succeeded)
            {
                await httpContext.ChallengeAsync();
                return;
            }

            await _next(httpContext);
        }


    }
}
