using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using System.Linq;
using TechCertain.Infrastructure.FluentNHibernate;

namespace TechCertain.Services.Impl
{
	public class OrganisationTypeService : IOrganisationTypeService
	{     
        IMapperSession<OrganisationType> _organisationTypeRepository;

        public OrganisationTypeService(IMapperSession<OrganisationType> organisationTypeRepository)
        {       
            _organisationTypeRepository = organisationTypeRepository;
        }
		#region IOrganisationTypeService implementation

		public void AddNew (string name)
		{
			throw new NotImplementedException ();
		}

		public IEnumerable<OrganisationType> GetOrganisationTypes ()
		{
			throw new NotImplementedException ();
		}

        public OrganisationType CreateNewOrganisationType(User user, string organisationTypeName)
        {
            OrganisationType OrganisationType = new OrganisationType(user, organisationTypeName);          
            _organisationTypeRepository.AddAsync(OrganisationType);

            return OrganisationType;
        }

        public OrganisationType GetOrganisationTypeByName(string organisationTypeName)
        {
            return _organisationTypeRepository.FindAll().FirstOrDefault(ot => ot.Name == organisationTypeName);
        }

        #endregion
    }
}

