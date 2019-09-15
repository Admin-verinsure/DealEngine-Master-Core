using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;


namespace DealEngine.Infrastructure.Identity.Subservices
{
    public class DealEngineSchemes : IAuthenticationSchemeProvider
    {
        public void AddScheme(AuthenticationScheme scheme)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AuthenticationScheme>> GetAllSchemesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationScheme> GetDefaultAuthenticateSchemeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationScheme> GetDefaultChallengeSchemeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationScheme> GetDefaultForbidSchemeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationScheme> GetDefaultSignInSchemeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationScheme> GetDefaultSignOutSchemeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AuthenticationScheme>> GetRequestHandlerSchemesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AuthenticationScheme> GetSchemeAsync(string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveScheme(string name)
        {
            throw new NotImplementedException();
        }
    }
}

