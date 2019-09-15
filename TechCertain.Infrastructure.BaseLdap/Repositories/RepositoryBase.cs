using TechCertain.Infrastructure.BaseLdap.Interfaces;

namespace TechCertain.Infrastructure.BaseLdap.Repositories
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

