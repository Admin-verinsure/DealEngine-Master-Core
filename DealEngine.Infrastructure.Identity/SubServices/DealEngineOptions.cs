using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DealEngine.Infrastructure.Identity.Subservices
{
    public class DealEngineOptions : IOptions<IdentityOptions>
    {
        public IdentityOptions Value => throw new NotImplementedException();
    }
}

