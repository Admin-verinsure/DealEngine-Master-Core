using DealEngine.Infrastructure.Legacy.Interfaces;
using DealEngine.Infrastructure.BaseLdap.Services;

namespace DealEngine.Infrastructure.Legacy.Services
{
    public class LegacySessionService : SessionService
    {
        public LegacySessionService(ILegacyLdapConfigService ldapConfigService) : base(ldapConfigService)
        {

        }
    }
}
