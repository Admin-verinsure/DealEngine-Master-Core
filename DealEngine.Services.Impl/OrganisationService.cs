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
			if(organisationId != Guid.Empty)
            {
				Organisation organisation = await _organisationRepository.GetByIdAsync(organisationId);
				// have a repo organisation? Return it
				if (organisation != null)
					return organisation;
				organisation = _ldapService.GetOrganisation(organisationId);
				// have a ldap organisation but no repo? Update NHibernate & return
				if (organisation != null)
				{
					await Update(organisation);
					return organisation;
				}
				throw new Exception("Organisation with id [" + organisationId + "] does not exist in the system");
			}
			return null;			
		}

		public async Task UpdateOrganisation(IFormCollection collection)
		{
			string TypeName = collection["OrganisationViewModel.InsuranceAttribute"].ToString();
			Organisation organisation;
			if (string.IsNullOrWhiteSpace(TypeName))
            {
				//Owner
				organisation = await UpdateOwner(collection);				
			}
            else
            {
				var user = await UpdateOrganisationUser(collection);
				var jsonOrganisation = (Organisation)GetModelDeserializedModel(typeof(Organisation), collection);
				organisation = await GetOrganisationByEmail(jsonOrganisation.Email);
				if(organisation != null)
                {					
					organisation = _mapper.Map(jsonOrganisation, organisation);
					if (organisation.OrganisationType.Name == "Person - Individual")
                    {
						organisation.Name = user.FirstName + " " + user.LastName;

					}
					UpdateOrganisationUnit(organisation, collection);
					UpdateInsuranceAttribute(organisation, collection);						
				}				
            }

			await Update(organisation);			
		}

        private void UpdateInsuranceAttribute(Organisation organisation, IFormCollection collection)
        {
			string TypeName = collection["OrganisationViewModel.InsuranceAttribute"].ToString();
			var IA = organisation.InsuranceAttributes.FirstOrDefault(i => i.Name == TypeName);
			if (IA == null)
			{
				organisation.InsuranceAttributes.Clear();
				organisation.InsuranceAttributes.Add(
					new InsuranceAttribute(null, TypeName)
					);
			}
		}

        private void UpdateOrganisationUnit(Organisation organisation, IFormCollection collection)
        {
			var UnitName = collection["Unit"].ToString();
			string TypeName = collection["OrganisationViewModel.InsuranceAttribute"].ToString();
			Type UnitType = Type.GetType(UnitName);
			var jsonUnit = (OrganisationalUnit)GetModelDeserializedModel(UnitType, collection);
			var unit = organisation.OrganisationalUnits.FirstOrDefault(ou => ou.GetType() == jsonUnit.GetType());
			if (unit != null)
			{
				_mapper.Map(jsonUnit, unit);
				unit.Name = TypeName;
			}
			else
			{
				unit = (OrganisationalUnit)Activator.CreateInstance(UnitType);
				_mapper.Map(jsonUnit, unit);
				unit.Name = TypeName;
				organisation.OrganisationalUnits.Add(unit);
			}
		}

        private async Task<User> UpdateOrganisationUser(IFormCollection collection)
        {			
			var jsonUser = (User)GetModelDeserializedModel(typeof(User), collection);
			var user = await _userService.GetUserByEmail(jsonUser.Email);
			if (user != null)
			{
				user = _mapper.Map(jsonUser, user);
				await _userService.Update(user);
			}
			return user;
		}

        private async Task<Organisation>  UpdateOwner(IFormCollection collection)
        {
			var jsonOrganisation = (Organisation)GetModelDeserializedModel(typeof(Organisation), collection);
			var organisation = await GetOrganisationByEmail(jsonOrganisation.Email);
			if(organisation != null)
            {
				organisation = _mapper.Map(jsonOrganisation, organisation);
				await Update(organisation);
			}
			return organisation;
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

		public async Task<List<Organisation>> GetNZFSGSubsystemAdvisors(ClientInformationSheet sheet)
		{
			var organisations = new List<Organisation>();			
			foreach (var organisation in sheet.Organisation.Where(o=>o.Removed != true && o.InsuranceAttributes.Any(i => i.Name == "Advisor")))
			{
				var unit = (AdvisorUnit)organisation.OrganisationalUnits.FirstOrDefault(u => u.Type == "Advisor" || u.Type == "Nominated Representative");
				if (unit != null)
				{
					organisations.Add(organisation);
				}
			}
			return organisations;
		}

		public async Task<List<Organisation>> GetTripleASubsystemAdvisors(ClientInformationSheet sheet)
		{
			var organisations = new List<Organisation>();
			foreach (var organisation in sheet.Organisation.Where(o => o.InsuranceAttributes.Any(i => i.Name == "Advisor" || i.Name == "Nominated Representative")))
			{
				var UnitName = organisation.InsuranceAttributes.FirstOrDefault().Name;
				var unit = (AdvisorUnit)organisation.OrganisationalUnits.FirstOrDefault(u => u.Name == UnitName);
				if (unit != null)
                {
					if(!unit.IsPrincipalAdvisor && organisation.Removed != true)
                    {
						organisations.Add(organisation);
					}					
				}				
			}
			return organisations;
		}


        public async Task<Organisation> CreateOrganisation(string Email, string Type, string OrganisationName, string OrganisationTypeName, string FirstName, string LastName, User Creator, IFormCollection collection)
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
				await _userService.Create(User);
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
				if(Type == "Advisor" || 
					Type == "Nominated Representative" ||
					Type == "Administration"
					)
                {
					OrganisationalUnits.Add(new OrganisationalUnit(User, "Private", OrganisationTypeName, collection));
					OrganisationalUnits.Add(new AdvisorUnit(User, Type, OrganisationTypeName, collection));
				}
				if (Type == "Personnel")
				{
					OrganisationalUnits.Add(new OrganisationalUnit(User, "Private", OrganisationTypeName,  collection));
					OrganisationalUnits.Add(new PersonnelUnit(User, Type, OrganisationTypeName, collection));
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
		public object? GetModelDeserializedModel(Type type, IFormCollection collection)
		{
			Dictionary<object, string> model = new Dictionary<object, string>();
			object obj = null;
			try
			{
				foreach (var Key in collection.Keys)
				{
					var value = Key.Split(".").ToList().LastOrDefault();
					var Field = type.GetProperty(value);
					if (Field != null)
					{
						var fieldType = Field.PropertyType;
						if (
							(fieldType == typeof(string)) ||
							(fieldType == typeof(int)) ||
							(fieldType == typeof(decimal)) ||
							(fieldType == typeof(bool)) ||
							(fieldType == typeof(DateTime?)) || 
							(fieldType == typeof(DateTime))
							)
						{
							if (model.ContainsKey(value))
							{
								model[value] = collection[Key].ToString();
							}
							else
							{
								model.Add(value, collection[Key].ToString());
							}
						}
					}
				}

				var JsonString = GetSerializedModel(model);
				obj = JsonConvert.DeserializeObject(JsonString, type,
					new JsonSerializerSettings()
					{
						MaxDepth = 1,
						ObjectCreationHandling = ObjectCreationHandling.Auto,
						ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
						NullValueHandling = NullValueHandling.Ignore,
						DateFormatHandling = DateFormatHandling.IsoDateFormat,
						FloatFormatHandling = FloatFormatHandling.DefaultValue,
						TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
					}); ;
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return obj;
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

  //      public async Task RefactorOrganisations(Guid programmeId)
  //      {
		//	//turn off IA Organisations
		//	//run once for Units
		//	//Run again to Create Attributes
		//	await PrincipalUnit();
		//	//turn off IA Organisations
		//	await PersonnelUnit();
		//	//await PMINZ();
		//	//await NZACS();
		//}

  //      private async Task PersonnelUnit()
  //      {
		//	int value = 0;
		//	var organisations = await _organisationRepository.FindAll().ToListAsync();
		//	var attributeList = organisations.Where(o => o.OrganisationalUnits.Any(T => T.Name == "Personnel"));			
		//	//var PersonnelOrg = organisations.Where(o => o.InsuranceAttributes.Any(i => i.InsuranceAttributeName == "project management personnel")).ToList();

		//	string Message = "";
		//	string Id;
		//	try
		//	{
		//		foreach (var organisation in attributeList)
		//		{
		//			value++;
		//			Id = organisation.Id.ToString();
		//			Message = "Start Id:" + Id;
  //                  organisation.InsuranceAttributes.Add(new InsuranceAttribute(null, "Personnel"));
  //                  Message = "Added Attribute";

  //                  //DateTime DateQualified = DateTime.MinValue;
  //                  //DateTime.TryParse(organisation.DateQualified, out DateQualified);
  //                  //bool IsCurrentMembershipPMINZ = false;
  //                  //string CurrentMembershipNo = organisation.CurrentMembershipNo;
  //                  //if (!string.IsNullOrWhiteSpace(organisation.CurrentMembershipNo))
  //                  //{
  //                  //    IsCurrentMembershipPMINZ = true;
  //                  //    CurrentMembershipNo = organisation.CurrentMembershipNo;
  //                  //}

  //                  //organisation.OrganisationalUnits.Add(
  //                  //                   new PersonnelUnit()
  //                  //                   {
  //                  //                       Name = "Personnel",
  //                  //                       Type = "Person - Individual",
  //                  //                       IsRegisteredLicensed = organisation.IsRegisteredLicensed,
  //                  //                       InsuredEntityRelation = organisation.InsuredEntityRelation,
  //                  //                       DateQualified = DateQualified,
  //                  //                       DesignLicensed = organisation.DesignLicensed,
  //                  //                       SiteLicensed = organisation.SiteLicensed,
  //                  //                       IsCurrentMembership = organisation.IsCurrentMembership,
  //                  //                       OtherCompanyName = organisation.OtherCompanyname,
  //                  //                       YearOfPractice = organisation.YearofPractice,
  //                  //                       Qualifications = organisation.Qualifications,
  //                  //                       JobTitle = organisation.JobTitle,
  //                  //                       ProfAffiliation = organisation.ProfAffiliation,
  //                  //                       IsInsuredRequired = organisation.IsInsuredRequired,
  //                  //                       IsContractorInsured = organisation.IsContractorInsured,
  //                  //                       CertType = organisation.CertType,
  //                  //                       MajorShareHolder = organisation.MajorShareHolder,
  //                  //                       IsCurrentMembershipPMINZ = IsCurrentMembershipPMINZ,
  //                  //                       CurrentMembershipNo = CurrentMembershipNo
  //                  //                   });

  //                  //organisation.OrganisationalUnits.Add(
  //                  //    new OrganisationalUnit()
  //                  //    {
  //                  //        Name = "Private",
  //                  //        Type = "Person - Individual",
  //                  //    });

  //                  Message = "Added Units";

  //                  await _organisationRepository.UpdateAsync(organisation);
		//			Message = "Updated Organisation";
		//			Console.WriteLine(Message);
		//			Console.WriteLine(value);
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		throw ex;
		//	}

		//	Console.WriteLine(value);
		//}

  //      private async Task PrincipalUnit()
  //      {
		//	int value = 0;
		//	var organisations = await _organisationRepository.FindAll().ToListAsync();
		//	var attributeList = organisations.Where(o => o.OrganisationalUnits.Any(T => T.Name == "Principal"));
		//	//var PrincipalOrg = organisations.Where(o => o.InsuranceAttributes.Any(i=>i.InsuranceAttributeName =="Principal")).ToList();			
		//	string Message = "";
		//	string Id;
		//	try
		//	{
  //              foreach (var organisation in attributeList)
  //              {
  //                  value++;
  //                  Id = organisation.Id.ToString();
  //                  Message = "Start Id:" + Id;
  //                  organisation.InsuranceAttributes.Add(new InsuranceAttribute(null, "Principal"));
  //                  Message = "Added Attribute";

  //                  //organisation.OrganisationalUnits.Add(
  //                  //    new PrincipalUnit()
  //                  //    {
  //                  //        Name = "Principal",
  //                  //        Type = "Person - Individual",
  //                  //        DateofRetirement = organisation.DateofRetirement,
  //                  //        IsRetiredorDeceased = organisation.IsRetiredorDecieved,
  //                  //        Qualifications = organisation.Qualifications,
  //                  //        IsIPENZmember = organisation.IsIPENZmember,
  //                  //        CPEngQualified = organisation.CPEngQualified,
  //                  //        YearOfPracticeCEAS = organisation.YearofPractice,
  //                  //        IsNZIAmember = organisation.IsNZIAmember,
  //                  //        IsADNZmember = organisation.IsADNZmember,
  //                  //        NZIAmembership = organisation.NZIAmembership,
  //                  //        IsLPBCategory3 = organisation.IsLPBCategory3,
  //                  //        IsOtherdirectorship = organisation.IsOtherdirectorship,
  //                  //        TradingName = organisation.TradingName,
  //                  //        YearOfPracticeNZACS = organisation.YearofPractice,
  //                  //        PrevPracticeNZACS = organisation.PrevPractice
  //                  //    });

  //                  //organisation.OrganisationalUnits.Add(
  //                  //    new OrganisationalUnit()
  //                  //    {
  //                  //        Name = "Private",
  //                  //        Type = "Person - Individual",
  //                  //    });

  //                  await _organisationRepository.UpdateAsync(organisation);
		//			Message = "Updated Organisation";
		//			Console.WriteLine(Message);
		//			Console.WriteLine(value);

		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		throw ex;
		//	}

		//	Console.WriteLine(value);
		//}
    }

}

