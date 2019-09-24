using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace DealEngine.Infrastructure.Identity
{
    public class DealEngineUserManager : UserManager<User>
    {
        IRepository<User> _userRepository;

        public DealEngineUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger, IRepository<User> userRepository) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userRepository = userRepository;
        }

        //public async Task<User> GetCurrentUserAsync()
        //{
        //    return await _userRepository.GetByIdAsync(_currentUserGuid);
        //}

    }
}

