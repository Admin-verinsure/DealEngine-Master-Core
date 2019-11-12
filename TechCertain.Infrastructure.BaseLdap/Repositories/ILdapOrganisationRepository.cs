using Novell.Directory.Ldap;
using System;
using TechCertain.Domain.Entities;


namespace TechCertain.Infrastructure.BaseLdap.Repositories
{
    public interface ILdapOrganisationRepository
    {
        Organisation Get(Guid organisationID);
        bool Create(Organisation organisation);
    }
}

