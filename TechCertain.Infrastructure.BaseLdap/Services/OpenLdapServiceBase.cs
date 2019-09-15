using Novell.Directory.Ldap;
using TechCertain.Infrastructure.BaseLdap.Interfaces;

namespace TechCertain.Infrastructure.BaseLdap.Services
{
	public class OpenLdapServiceBase
	{
		protected ILdapRepository _ldapRepository;
		protected ILdapConfigService _ldapConfigService;

		public OpenLdapServiceBase(ILdapRepository ldapRepository, ILdapConfigService ldapConfigService)
		{
			_ldapRepository = ldapRepository;
			_ldapConfigService = ldapConfigService;
		}

		protected LdapEntry GetLdapEntry(string dn)
		{
			return GetLdapEntry (dn, _ldapConfigService.AdminBindDN, _ldapConfigService.AdminBindPassword);
		}

		protected LdapEntry GetLdapEntry(string dn, string bindDN, string password)
		{
			return _ldapRepository.GetEntry (dn, bindDN, password);
		}
	}
}

