using Bismuth.Ldap;
using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.BaseLdap.Converters;
using TechCertain.Infrastructure.BaseLdap.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Infrastructure.Ldap.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
	public class OrganisationService : IOrganisationService
    {	
		IMapperSession<Organisation> _organisationRepository;
		ILdapService _ldapService;
        ILdapConfigService _ldapConfigService;

        public OrganisationService(IMapperSession<Organisation> organisationRepository, ILdapConfigService ldapConfigService, ILdapService ldapService)
        {
            _ldapConfigService = ldapConfigService;
            _organisationRepository = organisationRepository;
			_ldapService = ldapService;
		}

		public Organisation CreateNewOrganisation (Organisation organisation)
		{
			_ldapService.Create (organisation);
			UpdateOrganisation (organisation);
			return organisation;
		}

		public Organisation CreateNewOrganisation (string p1, OrganisationType organisationType, string ownerFirstName, string ownerLastName, string ownerEmail)
		{
			Organisation organisation = new Organisation (null, Guid.NewGuid (), p1, organisationType);
			// TODO - finish this later since I need to figure out what calls the controller function that calls this service function
			throw new NotImplementedException ();
		}

		public bool DeleteOrganisation (User deletedBy, Organisation organisation)
		{
			organisation.Delete (deletedBy);
			return UpdateOrganisation (organisation);
		}

		public IQueryable<Organisation> GetAllOrganisations ()
		{
			// we don't want to query ldap. That way lies timeouts. Or Dragons.
			return _organisationRepository.FindAll ();
		}

		public Organisation GetOrganisation (Guid organisationId)
		{
			Organisation organisation = _organisationRepository.GetById(organisationId).Result;
			// have a repo organisation? Return it
			if (organisation != null)
				return organisation;
			organisation = _ldapService.GetOrganisation (organisationId);
			// have a ldap organisation but no repo? Update NHibernate & return
			if (organisation != null) {
				UpdateOrganisation (organisation);
				return organisation;
			}
			// no organisation at all? Throw exception
			throw new Exception ("Organisation with id [" + organisationId + "] does not exist in the system");
		}

		public bool UpdateOrganisation (Organisation organisation)
		{
			_organisationRepository.AddAsync(organisation);
			_ldapService.Update (organisation);
			return true;
		}

        public Organisation GetOrganisationByName(string organisationName)
        {
            return _organisationRepository.FindAll().FirstOrDefault(o => o.Name == organisationName);
        }

        public Organisation GetOrganisationByEmail(string organisationEmail)
        {
            return _organisationRepository.FindAll().FirstOrDefault(o => o.Email == organisationEmail);
        }

        //public Organisation Get(Guid organisationID)
        //{
        //    string organisationDn = _ldapConfigService.GetOrganisationDN(organisationID);
        //    LdapEntry entry = GetLdapEntry(organisationDn);

        //    if (entry == null && _ldapConfigService.ImportOrganisation(organisationID))
        //        entry = GetLdapEntry(organisationDn);

        //    return LdapConverter.ToOrganisation(entry);
        //}


    }
}

