using Novell.Directory.Ldap;
using System;
using DealEngine.Domain.Entities;

namespace DealEngine.Domain.Services
{
	public interface IOrganisationRepository
	{
		Organisation Get(Guid organisationID);

        Organisation GetOrganisation(string organisationName);

        bool Create (Organisation organisation);

		bool Update (Organisation organisation);

		bool Delete (Organisation organisation);
        LdapEntry GetLdapEntry(string dn);
    }
}

