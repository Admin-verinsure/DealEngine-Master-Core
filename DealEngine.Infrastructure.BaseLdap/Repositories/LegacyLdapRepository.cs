using DealEngine.Infrastructure.BaseLdap.Interfaces;
using DealEngine.Infrastructure.BaseLdap.Repositories;
using DealEngine.Infrastructure.Legacy.Interfaces;

namespace DealEngine.Infrastructure.Legacy.Repositories
{
    public class LegacyLdapRepository : LdapRepository, ILegacyLdapRepository
    {
        public LegacyLdapRepository(ISessionService sessionService, ILegacyLdapConfigService configService) : base (sessionService, configService)
        {                   
        }
    }
}
