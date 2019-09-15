using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TechCertain.Domain.Entities;

namespace DealEngine.Infrastructure.Identity.Subservices
{
    public class DealEnginePasswordValidator : IEnumerable<IPasswordValidator<User>>
    {
        public IEnumerator<IPasswordValidator<User>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

