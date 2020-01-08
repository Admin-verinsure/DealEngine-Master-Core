using Bismuth.Ldap;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

		public async Task<Organisation> CreateNewOrganisation (Organisation organisation)
		{
			try
			{
				_ldapService.Create(organisation);
			}
			catch (Exception ex)
			{
				//org exists in LDap but not in application
				if(ex.HResult == 68)
				{
					await UpdateOrganisation(organisation);
				}				
			}
					
			return organisation;
		}

		public Organisation CreateNewOrganisation (string p1, OrganisationType organisationType, string ownerFirstName, string ownerLastName, string ownerEmail)
		{
			Organisation organisation = new Organisation (null, Guid.NewGuid (), p1, organisationType);
			// TODO - finish this later since I need to figure out what calls the controller function that calls this service function
			throw new NotImplementedException ();
		}

		public async Task DeleteOrganisation (User deletedBy, Organisation organisation)
		{
			organisation.Delete (deletedBy);
			await UpdateOrganisation(organisation);
		}

		public async Task<List<Organisation>> GetAllOrganisations ()
		{
			// we don't want to query ldap. That way lies timeouts. Or Dragons.
			return await _organisationRepository.FindAll().ToListAsync();
		}

		public async Task<Organisation> GetOrganisation (Guid organisationId)
		{
			Organisation organisation = await _organisationRepository.GetByIdAsync(organisationId);
			// have a repo organisation? Return it
			if (organisation != null)
				return organisation;
			organisation = _ldapService.GetOrganisation (organisationId);
			// have a ldap organisation but no repo? Update NHibernate & return
			if (organisation != null) {
				await UpdateOrganisation (organisation);
				return organisation;
			}
			// no organisation at all? Throw exception
			throw new Exception ("Organisation with id [" + organisationId + "] does not exist in the system");
		}

		public async Task UpdateOrganisation (Organisation organisation)
		{
			await _organisationRepository.AddAsync(organisation);
			_ldapService.Update (organisation);
		}

        public async Task<Organisation> GetOrganisationByName(string organisationName)
        {
            return await _organisationRepository.FindAll().FirstOrDefaultAsync(o => o.Name == organisationName);
        }

        public async Task<Organisation> GetOrganisationByEmail(string organisationEmail)
        {
            return await _organisationRepository.FindAll().FirstOrDefaultAsync(o => o.Email == organisationEmail);
        }
    }
}

