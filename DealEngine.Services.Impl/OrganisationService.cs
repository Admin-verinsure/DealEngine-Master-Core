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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHibernate.Mapping;

namespace DealEngine.Services.Impl
{
	public class OrganisationService : IOrganisationService
	{
		IMapperSession<Organisation> _organisationRepository;
		IOrganisationTypeService _organisationTypeService;
		IOrganisationalUnitService _organisationalUnitService;
		ILdapService _ldapService;
		IUserService _userService;
		IInsuranceAttributeService _insuranceAttributeService;
		ILogger<OrganisationService> _logger;

		public OrganisationService(IMapperSession<Organisation> organisationRepository,
			IUserService userService,
			IOrganisationalUnitService organisationalUnitService,
			IOrganisationTypeService organisationTypeService,
			ILdapService ldapService,
			IInsuranceAttributeService insuranceAttributeService,
			ILogger<OrganisationService> logger
			)
		{
			_logger = logger;
			_userService = userService;
			_organisationalUnitService = organisationalUnitService;
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
			await UpdateApplication(organisation);
			UpdateLDap(organisation);
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
			var insuranceAttribute = await _insuranceAttributeService.GetInsuranceAttributeByName("Advisor");
			foreach (var organisation in sheet.Organisation.Where(o=>o.Removed != true && o.InsuranceAttributes.Contains(insuranceAttribute) || o.Type == "Advisor"))
			{
				organisations.Add(organisation);
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


        public async Task<Organisation> GetOrCreateOrganisation(string Email, string Type, string OrganisationName, string OrganisationTypeName, string FirstName, string LastName, User Creator, IFormCollection collection)
        {
			Organisation foundOrg = await GetOrganisationByEmail(Email);
			if (foundOrg == null)
			{
				OrganisationalUnit OrganisationalUnit = null;

				var User = await _userService.GetUserByEmail(Email);				
				if (User != null)
				{					
					var SameUser = await _userService.GetUser(User.UserName);
					if (User != SameUser)
					{						
						User = new User(Creator, Guid.NewGuid(), collection);
					}
				}
                else
                {
					User = new User(Creator, Guid.NewGuid(), collection);
				}

				if (Type == "Private" || Type== "Advisor" || Type == "NominatedRepresentative")
				{
					OrganisationName = FirstName + " " + LastName;
					OrganisationTypeName = "Person - Individual";
					OrganisationalUnit = new OrganisationalUnit(User, "Home", collection);
				}
				else if (Type == "Company")
				{
					OrganisationTypeName = "Corporation – Limited liability";
					OrganisationalUnit = new OrganisationalUnit(User, "Head Office", collection);
				}
				else if (Type == "Trust")
				{
					OrganisationTypeName = "Trust";
					OrganisationalUnit = new OrganisationalUnit(User, "Head Office", collection);
				}
				else if (Type == "Trust")
				{
					OrganisationTypeName = "Trust";
					OrganisationalUnit = new OrganisationalUnit(User, "Head Office", collection);
				}
				else if (Type == "Partnership")
				{
					OrganisationTypeName = "Partnership";
					OrganisationalUnit = new OrganisationalUnit(User, "Head Office", collection);
				}

				OrganisationalUnit = await _organisationalUnitService.CreateOrganisationalUnit(OrganisationalUnit);
				OrganisationType OrganisationType = await _organisationTypeService.GetOrganisationTypeByName(OrganisationTypeName);
				InsuranceAttribute InsuranceAttribute = await _insuranceAttributeService.GetInsuranceAttributeByName(Type);
				foundOrg = CreateNewOrganisation(Creator, OrganisationName, OrganisationType, OrganisationalUnit, InsuranceAttribute);

				if (!User.Organisations.Contains(foundOrg))
					User.Organisations.Add(foundOrg);

				User.SetPrimaryOrganisation(foundOrg);
				await _userService.Update(User);
			}
			return foundOrg;			
        }

        private Organisation CreateNewOrganisation(User Creator, string organisationName, OrganisationType organisationType, OrganisationalUnit organisationalUnit, InsuranceAttribute insuranceAttribute)
        {
			var Organisation =  new Organisation(Creator, Guid.NewGuid(), organisationName, organisationType, organisationalUnit, insuranceAttribute);
			return Organisation;
        }

        public async Task<Organisation> GetAnyRemovedAdvisor(string email)
        {
			var advisoryAttr = await _insuranceAttributeService.GetInsuranceAttributeByName("Advisor");
			var organisations = await GetAllOrganisationsByEmail(email);
			var organisation = organisations.FirstOrDefault(o => o.InsuranceAttributes.Contains(advisoryAttr) && o.Removed == true);
			return organisation;
		}

        public async Task ChangeOwner(Organisation organisation, ClientInformationSheet sheet)
        {
			if(sheet != null)
            {
                try
                {
					var insuranceattribute = organisation.InsuranceAttributes.FirstOrDefault(IA => IA.InsuranceAttributeName == organisation.Type);
					insuranceattribute.SetHistory(sheet);
				}
				catch(Exception ex)
                {
					throw ex;
                }
				
			}
			organisation.IsPrincipalAdvisor = true;
			organisation.Removed = false;
			await UpdateOrganisation(organisation);
		}

        public async  Task UpdateApplication(Organisation organisation)
        {
			await _organisationRepository.AddAsync(organisation);
		}

        private void UpdateLDap(Organisation organisation)
        {
			_ldapService.Update(organisation);
		}

        public async Task Update(Organisation organisation)
        {
			try
			{
				UpdateLDap(organisation);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex.Message);
			}
			finally
			{
				await _organisationRepository.UpdateAsync(organisation);
			}
		}
    }

}

