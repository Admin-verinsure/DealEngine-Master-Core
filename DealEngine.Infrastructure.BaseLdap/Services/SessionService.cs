using System;
using Novell.Directory.Ldap;
using DealEngine.Infrastructure.BaseLdap.Interfaces;

namespace DealEngine.Infrastructure.BaseLdap.Services
{
	public class SessionService : ISessionService
	{
		private ILdapConfigService _ldapConfigService;

		public SessionService(ILdapConfigService ldapConfigService)
		{
            _ldapConfigService = ldapConfigService;
		}

		public LdapConnection GetConnection ()
		{
			return GetConnection(_ldapConfigService.ServerAddress, _ldapConfigService.ServerPort);
		}

		LdapConnection GetConnection (string host, int port)
		{
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException("host", "Can not create a LdapConnection with no host / server address.");

			LdapConnection ldapConnection = new LdapConnection ();
			ldapConnection.Connect (host, port);

            

			return ldapConnection;
		}
	}
}

