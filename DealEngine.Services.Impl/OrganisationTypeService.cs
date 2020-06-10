using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using System.Linq;
using DealEngine.Infrastructure.FluentNHibernate;
using NHibernate.Linq;
using System.Threading.Tasks;

namespace DealEngine.Services.Impl
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

        public async Task<OrganisationType> CreateNewOrganisationType(User user, string organisationTypeName)
        {
            OrganisationType OrganisationType = null;
            if (user != null)
            {
                OrganisationType = new OrganisationType(user, organisationTypeName);
            }
            else
            {
                OrganisationType = new OrganisationType(organisationTypeName);
            }
                    
            await _organisationTypeRepository.AddAsync(OrganisationType);

            return OrganisationType;
        }

        public async Task<OrganisationType> GetOrganisationTypeByName(string organisationTypeName)
        {
            OrganisationType GetOrganisationType = await _organisationTypeRepository.FindAll().FirstOrDefaultAsync(ot => ot.Name == organisationTypeName);
            if(GetOrganisationType == null)
            {
                GetOrganisationType = await CreateNewOrganisationType(null, organisationTypeName);
            }
            return GetOrganisationType;
        }

        #endregion
    }
}

