using TechCertain.Infrastructure.BaseLdap.Interfaces;
using TechCertain.Infrastructure.BaseLdap.Repositories;
using TechCertain.Infrastructure.Legacy.Interfaces;

namespace TechCertain.Infrastructure.Legacy.Repositories
{
    public class LegacyLdapRepository : LdapRepository, ILegacyLdapRepository
    {
        public LegacyLdapRepository(ISessionService sessionService, ILegacyLdapConfigService configService) : base (sessionService, configService)
        {                   
        }
    }
}
