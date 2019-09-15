using System;
using TechCertain.Infrastructure.BaseLdap.Interfaces;

namespace TechCertain.Infrastructure.Legacy.Interfaces
{
    public interface ILegacyLdapConfigService : ILdapConfigService
    {
        string[] OrganisationAttributes { get; }

        string GetOrganisationSearchDN(Guid organisationID);
    }
}
