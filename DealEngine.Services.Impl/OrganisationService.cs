using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.BaseLdap.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Infrastructure.Ldap.Interfaces;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
	public class OrganisationService : IOrganisationService
	{
		IMapperSession<Organisation> _organisationRepository;
		ILdapService _ldapService;		
		IInsuranceAttributeService _insuranceAttributeService;

		public OrganisationService(IMapperSession<Organisation> organisationRepository, 			
			ILdapService ldapService,
			IInsuranceAttributeService insuranceAttributeService)
		{
			_insuranceAttributeService = insuranceAttributeService;			
			_organisationRepository = organisationRepository;
			_ldapService = ldapService;
		}

		public async Task<Organisation> CreateNewOrganisation(Organisation organisation)
		{
			try
			{
				await UpdateOrganisation(organisation);
				_ldapService.Create(organisation);
			}
			catch (Exception ex)
			{
				//org exists in LDap but not in application
				if (ex.HResult == 68)
				{
					await UpdateOrganisation(organisation);
				}
			}
			
			return organisation;
		}

		public Organisation CreateNewOrganisation(string p1, OrganisationType organisationType, string ownerFirstName, string ownerLastName, string ownerEmail)
		{
			Organisation organisation = new Organisation(null, Guid.NewGuid(), p1, organisationType);
			// TODO - finish this later since I need to figure out what calls the controller function that calls this service function
			throw new NotImplementedException();
		}

		public async Task DeleteOrganisation(User deletedBy, Organisation organisation)
		{
			organisation.Delete(deletedBy);
			await UpdateOrganisation(organisation);
		}

		public async Task<List<Organisation>> GetAllOrganisations()
		{
			// we don't want to query ldap. That way lies timeouts. Or Dragons.
			return await _organisationRepository.FindAll().ToListAsync();
		}

		public async Task<Organisation> GetOrganisation(Guid organisationId)
		{
			Organisation organisation = await _organisationRepository.GetByIdAsync(organisationId);
			// have a repo organisation? Return it
			if (organisation != null)
				return organisation;
			organisation = _ldapService.GetOrganisation(organisationId);
			// have a ldap organisation but no repo? Update NHibernate & return
			if (organisation != null) {
				await UpdateOrganisation(organisation);
				return organisation;
			}
			// no organisation at all? Throw exception
			throw new Exception("Organisation with id [" + organisationId + "] does not exist in the system");
		}

		public async Task UpdateOrganisation(Organisation organisation)
		{
			await _organisationRepository.AddAsync(organisation);
			_ldapService.Update(organisation);
		}

		public async Task<Organisation> GetOrganisationByName(string organisationName)
		{
			return await _organisationRepository.FindAll().FirstOrDefaultAsync(o => o.Name == organisationName);
		}

		public async Task<Organisation> GetOrganisationByEmail(string organisationEmail)
		{
			return await _organisationRepository.FindAll().FirstOrDefaultAsync(o => o.Email == organisationEmail);
		}

		public async Task<List<Organisation>> GetOrganisationPrincipals(ClientInformationSheet sheet)
		{
			var organisations = new List<Organisation>();
			var Insurancelist = await _insuranceAttributeService.GetInsuranceAttributes();
			foreach (InsuranceAttribute IA in Insurancelist.Where(ia => ia.InsuranceAttributeName == "Principal"
				|| ia.InsuranceAttributeName == "Subsidiary"
				|| ia.InsuranceAttributeName == "PreviousConsultingBusiness"
				|| ia.InsuranceAttributeName == "JointVenture"
				|| ia.InsuranceAttributeName == "Mergers"
				|| ia.InsuranceAttributeName == "Advisor"
				|| ia.InsuranceAttributeName == "NominatedRepresentative"
				|| ia.InsuranceAttributeName == "project management personnel"))
			{
				foreach (var org in IA.IAOrganisations)
				{
					foreach (var organisation in sheet.Organisation.Where(o => o.Id == org.Id && o.Removed != true))
					{
						organisations.Add(organisation);
					}
				}
			}
			return organisations;
		}

	}
	
}

