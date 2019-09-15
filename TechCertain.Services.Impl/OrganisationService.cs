using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Infrastructure.Ldap.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
	public class OrganisationService : IOrganisationService
    {
		IUnitOfWorkFactory _unitOfWork;
		ILogger _logging;
		IRepository<Organisation> _organisationRepository;
		ILdapService _ldapService;

		public OrganisationService(IUnitOfWorkFactory unitOfWork, ILogger logging, IRepository<Organisation> organisationRepository, ILdapService ldapService)
        {
			_unitOfWork = unitOfWork;
			_logging = logging;
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
			Organisation organisation = _organisationRepository.GetById (organisationId);
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
			_organisationRepository.Add (organisation);
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


        //public void CreateOrganization(User user)
        //{
        //    CreateDefaultUserOrganisation(user);
        //    _ldapService.Create(user);
        //    Update(user);
        //}

        //protected void CreateDefaultUserOrganisation(User user)
        //{
        //    OrganisationType personalOrganisationType = null;
        //    personalOrganisationType = OrganisationType.GetOrganisationTypeByName("personal");
        //    if (personalOrganisationType == null)
        //    {
        //        personalOrganisationType = new OrganisationType(user, "personal");
        //    }
        //    Organisation defaultOrganisation = Organisation.CreateDefaultOrganisation(user, user, personalOrganisationType);
        //    user.Organisations.Add(defaultOrganisation);
        //}
    }
}

