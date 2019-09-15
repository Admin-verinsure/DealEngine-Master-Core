using TechCertain.Infrastructure.Legacy.Interfaces;
using TechCertain.Infrastructure.BaseLdap.Services;

namespace TechCertain.Infrastructure.Legacy.Services
{
    public class LegacySessionService : SessionService
    {
        public LegacySessionService(ILegacyLdapConfigService ldapConfigService) : base(ldapConfigService)
        {

        }
    }
}
