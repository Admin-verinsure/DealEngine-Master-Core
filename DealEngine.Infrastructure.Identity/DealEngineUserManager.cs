using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechCertain.Domain.Entities;

namespace DealEngine.Infrastructure.Identity
{
    public class DealEngineUserManager : UserManager<User>
    {
        protected IUserStore<User> Store { get; set; }
        protected IOptions<IdentityOptions> OptionsAccessor { get; set; }
        protected IPasswordHasher<User> PasswordHasher { get; set; }
        protected IEnumerable<IUserValidator<User>> UserValidators { get; set; }
        protected IEnumerable<IPasswordValidator<User>> PasswordValidators { get; set; }
        protected ILookupNormalizer KeyNormalizer { get; set; }
        protected IdentityErrorDescriber Errors { get; set; }
        protected IServiceProvider Services { get; set; }
        protected ILogger<UserManager<User>> Logger { get; set; }

        public DealEngineUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) 
            : base(store, optionsAccessor, passwordHasher, null, null, keyNormalizer, errors, services, logger)
        {
            Store = store;
            OptionsAccessor = optionsAccessor;
            PasswordHasher = passwordHasher;
            UserValidators = userValidators;
            PasswordValidators = passwordValidators;
            Errors = errors;
            KeyNormalizer = keyNormalizer;
            Services = services;
            Logger = logger;
        }
    }
}

