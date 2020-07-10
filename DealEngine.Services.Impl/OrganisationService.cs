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
using AutoMapper;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DealEngine.Services.Impl
{
	public class OrganisationService : IOrganisationService
	{
		IMapperSession<Organisation> _organisationRepository;
		IOrganisationTypeService _organisationTypeService;
		ILdapService _ldapService;
		IUserService _userService;
		IInsuranceAttributeService _insuranceAttributeService;
		ILogger<OrganisationService> _logger;
		IMapper _mapper;

		public OrganisationService(IMapperSession<Organisation> organisationRepository,
			IMapper mapper,
			IUserService userService,
			IOrganisationTypeService organisationTypeService,
			ILdapService ldapService,
			IInsuranceAttributeService insuranceAttributeService,
			ILogger<OrganisationService> logger
			)
		{
			_mapper = mapper;
			_logger = logger;
			_userService = userService;
			_organisationTypeService = organisationTypeService;
			_insuranceAttributeService = insuranceAttributeService;			
			_organisationRepository = organisationRepository;
			_ldapService = ldapService;
		}

		public async Task<Organisation> CreateNewOrganisation(Organisation organisation)
		{
			try
			{
				await Update(organisation);
				_ldapService.Create(organisation);
			}
			catch (Exception ex)
			{
				//org exists in LDap but not in application
				if (ex.HResult == 68)
				{
					//await Update(organisation);
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
			await Update(organisation);
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
				await Update(organisation);
				return organisation;
			}
			// no organisation at all? Throw exception
			throw new Exception("Organisation with id [" + organisationId + "] does not exist in the system");
		}

		public async Task UpdateOrganisation(IFormCollection collection)
		{
			var UnitName = collection["Unit"].ToString();
			Type UnitType = Type.GetType(UnitName);
			var jsonOrganisation = (Organisation)GetModelDeserializedModel(typeof(Organisation), collection);
			var jsonUser = (User)GetModelDeserializedModel(typeof(User), collection);
			var jsonUnit = (OrganisationalUnit)GetModelDeserializedModel(UnitType, collection);

			var user = await _userService.GetUserByEmail(jsonUser.Email);
			if(user != null)
            {
				user = _mapper.Map(jsonUser, user);
				await _userService.Update(user);
			}

			var organisation = await GetOrganisationByEmail(jsonOrganisation.Email);			
			if(organisation != null)
            {
				organisation = _mapper.Map(jsonOrganisation, organisation);
				var unit = organisation.OrganisationalUnits.FirstOrDefault(u => u.Name == jsonUnit.Type);
				if(unit != null)
                {
					_mapper.Map(jsonUnit, unit);
				}
                else
                {
					organisation.OrganisationalUnits.Add(jsonUnit);
					organisation.InsuranceAttributes.Add(new InsuranceAttribute(null, collection["Type"]));
					organisation.OrganisationType = new OrganisationType(null, collection["OrganisationType"]);
				}
				
				await Update(organisation);
			}			
			//UpdateLDap(organisation);
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

				if(string.IsNullOrWhiteSpace(OrganisationName))
                {
					OrganisationName = FirstName + " " + LastName;
					OrganisationTypeName = "Person - Individual";
				}

				List<OrganisationalUnit> OrganisationalUnits = GetOrganisationCreateUnits(Type, Creator, collection);
				//IList<OrganisationalUnit> OrganisationalUnit = await _organisationalUnitService.CreateOrganisationalUnit(OrganisationalUnit);
				OrganisationType OrganisationType = await _organisationTypeService.GetOrganisationTypeByName(OrganisationTypeName);
				InsuranceAttribute InsuranceAttribute = await _insuranceAttributeService.GetInsuranceAttributeByName(Type);
				foundOrg = CreateNewOrganisation(Creator, Email, OrganisationName, OrganisationType, OrganisationalUnits, InsuranceAttribute);

				if (!User.Organisations.Contains(foundOrg))
					User.Organisations.Add(foundOrg);

				User.SetPrimaryOrganisation(foundOrg);
				await _userService.Update(User);
			}
			return foundOrg;			
        }

        private List<OrganisationalUnit> GetOrganisationCreateUnits(string Type, User User, IFormCollection collection)
        {
			List<OrganisationalUnit> OrganisationalUnits = new List<OrganisationalUnit>();			
			string OrganisationTypeName;
			if (Type == "Company")
			{
				OrganisationTypeName = "Corporation – Limited liability";
				OrganisationalUnits.Add(new OrganisationalUnit(User, "Head Office", OrganisationTypeName, collection));
			}
			else if (Type == "Trust")
			{
				OrganisationTypeName = "Trust";
				OrganisationalUnits.Add(new OrganisationalUnit(User, "Head Office", OrganisationTypeName, collection));
			}
			else if (Type == "Partnership")
			{
				OrganisationTypeName = "Partnership";
				OrganisationalUnits.Add(new OrganisationalUnit(User, "Head Office", OrganisationTypeName, collection));
			}
			else
			{
				OrganisationTypeName = "Person - Individual";
				if (Type == "Private")
				{										
					OrganisationalUnits.Add(new OrganisationalUnit(User, Type, OrganisationTypeName, collection));
				}
				if(Type == "Advisor" || Type == "NominatedRepresentative")
                {
					OrganisationalUnits.Add(new OrganisationalUnit(User, "Private", OrganisationTypeName, collection));
					OrganisationalUnits.Add(new AdvisorUnit(User, Type, OrganisationTypeName, collection));
				}
				if (Type == "Personnel")
				{
					OrganisationalUnits.Add(new OrganisationalUnit(User, "Private", OrganisationTypeName,  collection));
					OrganisationalUnits.Add(new PersonnelUnit(User, Type, OrganisationTypeName, collection));
				}
				if (Type == "ProjectPersonnel")
				{
					OrganisationalUnits.Add(new OrganisationalUnit(User, "Private", OrganisationTypeName, collection));
					OrganisationalUnits.Add(new ProjectPersonnelUnit(User, Type, OrganisationTypeName, collection));
				}
				if(Type == "Principal")
                {
					OrganisationalUnits.Add(new OrganisationalUnit(User, "Private", OrganisationTypeName, collection));
					OrganisationalUnits.Add(new PrincipalUnit(User, Type, OrganisationTypeName, collection));
				}
			}

			return OrganisationalUnits;
		}

        private Organisation CreateNewOrganisation(User Creator, string email, string organisationName, OrganisationType organisationType, List<OrganisationalUnit> organisationalUnits, InsuranceAttribute insuranceAttribute)
        {
			var Organisation =  new Organisation(Creator, Guid.NewGuid(), organisationName, organisationType, organisationalUnits, insuranceAttribute, email);
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
			await Update(organisation);
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
		private object? GetModelDeserializedModel(Type type, IFormCollection collection)
		{
			Dictionary<object, string> model = new Dictionary<object, string>();
			//var Keys = collection.Keys.Where(s => s.StartsWith(ModelName + "." + type.Name, StringComparison.CurrentCulture));
			foreach (var Key in collection.Keys)
			{
				//model.Add(Key, collection[Key].ToString());
				var value = Key.Split(".").ToList().LastOrDefault();
				model.Add(value, collection[Key].ToString());
			}
			var JsonString = GetSerializedModel(model);
			try
			{
				var obj = JsonConvert.DeserializeObject(JsonString, type,
					new JsonSerializerSettings()
					{
						ObjectCreationHandling = ObjectCreationHandling.Auto,
						ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
						NullValueHandling = NullValueHandling.Ignore,
						DateFormatHandling = DateFormatHandling.IsoDateFormat,
						FloatFormatHandling = FloatFormatHandling.DefaultValue,
					});
				return obj;
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

		private string GetSerializedModel(object model)
		{
			try
			{
				return JsonConvert.SerializeObject(model,
					new JsonSerializerSettings()
					{
						// MaxDepth = 2,
						ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
						NullValueHandling = NullValueHandling.Ignore,
						FloatFormatHandling = FloatFormatHandling.DefaultValue,
						DateParseHandling = DateParseHandling.DateTime
					});
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

		}

        public async Task RefactorOrganisations(Guid programmeId)
        {
			int value = 0;
			var Principal = await _insuranceAttributeService.GetInsuranceAttributeByName("Principal");
			var organisations = await _organisationRepository.FindAll().ToListAsync();
			var CeasOrg = organisations.Where(o => o.InsuranceAttributes.Contains(Principal));
			foreach (var organisation in CeasOrg)
			{
                organisation.OrganisationalUnits.Add(
                    new PrincipalUnit()
                    {
                        Name = "Principal",
                        Type = "Person - Individual",
                        DateofRetirement = organisation.DateofRetirement,
                        IsRetiredorDeceased = organisation.IsRetiredorDecieved,
                        Qualifications = organisation.Qualifications,
                        IsIPENZmember = organisation.IsIPENZmember,
                        CPEngQualified = organisation.CPEngQualified,
                        YearofPracticeCEAS = organisation.YearofPractice,
                        IsNZIAmember = organisation.IsNZIAmember,
                        IsADNZmember = organisation.IsADNZmember,
                        NZIAmembership = organisation.NZIAmembership,
                        IsLPBCategory3 = organisation.IsLPBCategory3,
                        IsOtherdirectorship = organisation.IsOtherdirectorship,
                        TradingName = organisation.TradingName
                    });

                organisation.OrganisationalUnits.Add(
                    new OrganisationalUnit()
                    {
                        Name = "Private",
                        Type = "Person - Individual",
                    });

                await _organisationRepository.UpdateAsync(organisation);
				Console.WriteLine(value);
			}
			Console.WriteLine(value);
        }
    }

}

