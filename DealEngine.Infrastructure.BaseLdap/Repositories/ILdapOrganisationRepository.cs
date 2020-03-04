using Novell.Directory.Ldap;
using System;
using DealEngine.Domain.Entities;


namespace DealEngine.Infrastructure.BaseLdap.Repositories
{
    public interface ILdapOrganisationRepository
    {
        Organisation Get(Guid organisationID);
        bool Create(Organisation organisation);
    }
}

