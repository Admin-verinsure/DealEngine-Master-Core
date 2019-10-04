using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace DealEngine.Infrastructure.Identity.Data
{
    public class DealEngineUserManager : UserManager<DealEngineUser>
    {
        IMapperSession<User> _userRepository;

        public DealEngineUserManager(IUserStore<DealEngineUser> store, 
            IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<DealEngineUser> passwordHasher, 
            IEnumerable<IUserValidator<DealEngineUser>> userValidators, IEnumerable<IPasswordValidator<DealEngineUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<DealEngineUser>> logger, 
            IMapperSession<User> userRepository) 
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

