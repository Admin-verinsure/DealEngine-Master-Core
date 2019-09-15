using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TechCertain.Domain.Entities;

namespace DealEngine.Infrastructure.Identity.Subservices
{
    public class DealEngineValidators : IEnumerable<IUserValidator<User>>
    {
        public IEnumerator<IUserValidator<User>> GetEnumerator()
        {
            return null;
        }

        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }
    }
}

