using System;
using SimpleInjector;
using DealEngine.Infrastructure.Identity;
using Microsoft.AspNet.Identity;
using TechCertain.Domain.Entities;

namespace TechCertain.Infrastructure.DependecyResolution
{
	public class IdentityPackage//: IPackage
	{
		public void RegisterServices (Container container)
		{
			// See TechCertain.Infrastructure.Identity.SignInFactory for the following
			//container.Register (typeof (IUserStore<User, Guid>), typeof (NHibernateUserStore));
			//container.Register<IDealEngineRoleStore, NHibernateRoleStore> ();
			//container.Register<IDealEngineUserStore, NHibernateUserStore> ();
			//container.Register (typeof (UserManager<User, Guid>), typeof (DealEngineUserManager));

			RegisterTranisent (container, typeof (IUserStore<User, Guid>), typeof (NHibernateUserStore));
			RegisterTranisent (container, typeof (IDealEngineRoleStore), typeof (NHibernateRoleStore));
			RegisterTranisent (container, typeof (IDealEngineUserStore), typeof (NHibernateUserStore));
			RegisterTranisent (container, typeof (UserManager<User, Guid>), typeof (DealEngineUserManager));

			container.Register<IAuthenticationManager, CookieAuthenticationManager> ();
			container.Register<ISignInManager, DealEngineSignInManager> ();
		}

		void RegisterTranisent (Container container, Type tService, Type tImplementation)
		{
            //Registration registration = Lifestyle.Scoped.CreateRegistration(tService, tImplementation, container);
            //         registration.SuppressDiagnosticWarning (DiagnosticType.DisposableTransientComponent, "suppres");

            //var registration = Lifestyle.CreateRegistration(container, tService, tImplementation, container);
            //registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "suppres");

            //container.AddRegistration (tService, registration);
		}
	}
}

