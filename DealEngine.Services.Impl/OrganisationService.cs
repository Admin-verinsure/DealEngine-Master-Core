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
		IOrganisationTypeService _organisationTypeService;
		ILdapService _ldapService;		
		IInsuranceAttributeService _insuranceAttributeService;

		public OrganisationService(IMapperSession<Organisation> organisationRepository,
			IOrganisationTypeService organisationTypeService,
			ILdapService ldapService,
			IInsuranceAttributeService insuranceAttributeService)
		{
			_organisationTypeService = organisationTypeService;
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

		public Organisation CreateNewOrganisation(string organisationName, OrganisationType organisationType, string ownerFirstName, string ownerLastName, string ownerEmail)
		{
			Organisation organisation = new Organisation(null, Guid.NewGuid(), organisationName, organisationType);
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

		public async Task<List<Organisation>> GetAllOrganisationsByEmail(string email)
		{
			return await _organisationRepository.FindAll().Where(o => o.Email == email).ToListAsync();
		}

		public async Task<Organisation> GetExistingOrganisationByEmail(string organisationEmail)
		{
			return await _organisationRepository.FindAll().FirstOrDefaultAsync(o => o.Email == organisationEmail && o.Removed==true);
		}

		public async Task<List<Organisation>> GetOrganisationPrincipals(ClientInformationSheet sheet)
		{
			var organisations = new List<Organisation>();
			var Insurancelist = await _insuranceAttributeService.GetInsuranceAttributes();
			foreach (InsuranceAttribute IA in Insurancelist.Where(ia => ia.InsuranceAttributeName == "Advisor"))
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

		public async Task<List<Organisation>> GetSubsystemOrganisationPrincipals(ClientInformationSheet sheet)
		{
			var organisations = new List<Organisation>();
			var Insurancelist = await _insuranceAttributeService.GetInsuranceAttributes();
			foreach (InsuranceAttribute IA in Insurancelist.Where(ia => ia.InsuranceAttributeName == "Advisor"))
			{
				foreach (var org in IA.IAOrganisations)
				{
					foreach (var organisation in sheet.Organisation.Where(o => o.Id == org.Id && o.Removed != true && o.IsPrincipalAdvisor != true))
					{
						organisations.Add(organisation);
					}
				}
			}
			return organisations;
		}


        //      public async Task<List<Organisation>> GetOrCreateOrganisation(string Email)
        //      {
        //	User User = null;
        //	string OrganisationTypeName = "";
        //	var foundOrgs = await GetAllOrganisationsByEmail(Email);
        //          if (foundOrgs.Any())
        //          {
        //		return foundOrgs;
        //	}
        //          else
        //          {
        //		string ownerLastName = null;
        //		string ownerEmail = null;
        //		string ownerFirstName = null;
        //		string organisationName = null;
        //		string OrganisationTypeName = "";
        //		OrganisationType OrganisationType = _organisationTypeService.GetOrganisationTypeByName(OrganisationType);
        //		CreateNewOrganisation(organisationName, OrganisationType, ownerLastName, ownerLastName, Email);
        //          }

        //	if (orgType == "Private") //orgType = "Private", "Company", "Trust", "Partnership"
        //	{
        //		organisationName = firstName + " " + lastName;
        //		ouname = "Home";
        //	}
        //	else
        //	{
        //		ouname = "Head Office";
        //	}
        //	switch (orgType)
        //	{
        //		case "Private":
        //			{
        //				orgTypeName = "Person - Individual";
        //				break;
        //			}
        //		case "Company":
        //			{
        //				orgTypeName = "Corporation – Limited liability";
        //				break;
        //			}
        //		case "Trust":
        //			{
        //				orgTypeName = "Trust";
        //				break;
        //			}
        //		case "Partnership":
        //			{
        //				orgTypeName = "Partnership";
        //				break;
        //			}
        //		default:
        //			{
        //				throw new Exception(string.Format("Invalid Organisation Type: ", orgType));
        //			}
        //	}
        //	string phonenumber = null;

        //	phonenumber = mobilePhone;
        //	OrganisationType organisationType = null;
        //	organisationType = await _organisationTypeService.GetOrganisationTypeByName(orgTypeName);
        //	if (organisationType == null)
        //	{
        //		organisationType = await _organisationTypeService.CreateNewOrganisationType(null, orgTypeName);
        //	}

        //	organisation = new Organisation(currentUser, Guid.NewGuid(), organisationName, organisationType);
        //	organisation.Phone = phonenumber;
        //	organisation.Email = email;
        //	await _organisationService.CreateNewOrganisation(organisation);
        //}
    }

}

