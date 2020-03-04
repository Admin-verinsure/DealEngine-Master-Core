using System;
using DealEngine.Infrastructure.BaseLdap.Interfaces;

namespace DealEngine.Infrastructure.Legacy.Interfaces
{
    public interface ILegacyLdapConfigService : ILdapConfigService
    {
        string[] OrganisationAttributes { get; }

        string GetOrganisationSearchDN(Guid organisationID);
    }
}
