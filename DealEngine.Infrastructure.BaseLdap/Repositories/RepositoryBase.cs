using DealEngine.Infrastructure.BaseLdap.Interfaces;

namespace DealEngine.Infrastructure.BaseLdap.Repositories
{
	public class RepositoryBase
	{
		protected ILdapConfigService _ldapConfigService;

		public RepositoryBase (ILdapConfigService ldapConfigService)
		{
            _ldapConfigService = ldapConfigService;
		}
	}
}

