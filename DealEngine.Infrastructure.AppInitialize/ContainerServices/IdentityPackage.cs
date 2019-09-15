using System;
using System.Collections.Generic;
using DealEngine.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using TechCertain.Domain.Entities;
using Microsoft.Extensions.Options;
using DealEngine.Infrastructure.Identity.Subservices;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace DealEngine.Infrastructure.AppInitialize.ContainerServices
{
    public static class IdentityPackage
    {

        public static void RegisterServices(Container container)
        {
            //use this method when setup complete
            //container.Register(typeof(Microsoft.AspNetCore.Identity.IUserStore<User>), typeof(NHibernateUserStore), Lifestyle.Transient);

            container.Register(typeof(IUserStore<User>), typeof(NHibernateUserStore));
            container.Register(typeof(IDealEngineRoleStore), typeof(NHibernateRoleStore));
            container.Register(typeof(IDealEngineUserStore), typeof(NHibernateUserStore));
            container.Register(typeof(IPasswordHasher<User>), typeof(PasswordHasher<User>));
            container.Register(typeof(IUserValidator<User>), typeof(UserValidator<User>));
            container.Register(typeof(ILookupNormalizer), typeof(UpperInvariantLookupNormalizer));
            container.Register(typeof(UserManager<User>), typeof(DealEngineUserManager));

            container.Register(typeof(IUserClaimsPrincipalFactory<User>), typeof(DealEngineClaimsFactory));
            container.Register(typeof(ISignInManager), typeof(DealEngineSignInManager));
            container.Register<IAuthenticationManager, CookieAuthenticationManager>();

            RegisterDisposableTransient(container);
        }

        static void RegisterDisposableTransient(Container container)
        {

            Registration registration = container.GetRegistration(typeof(IUserStore<User>)).Registration;
            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent,
                "Ignore");

            Registration registration1 = container.GetRegistration(typeof(IDealEngineUserStore)).Registration;
            registration1.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent,
                "Ignore");

            Registration registration2 = container.GetRegistration(typeof(DealEngineUserManager)).Registration;
            registration2.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent,
                "Ignore");

            Registration registration3 = container.GetRegistration(typeof(UserManager<User>)).Registration;
            registration3.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent,
                "Ignore");

            Registration registration4 = container.GetRegistration(typeof(NHibernateRoleStore)).Registration;
            registration4.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent,
                "Ignore");

            Registration registration5 = container.GetRegistration(typeof(IDealEngineRoleStore)).Registration;
            registration5.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent,
                "Ignore");
        }
    }
}
