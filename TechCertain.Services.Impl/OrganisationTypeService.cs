using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using System.Linq;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Services.Impl
{
	public class OrganisationTypeService : IOrganisationTypeService
	{

        IUnitOfWorkFactory _unitOfWork;
        ILogger _logging;
        IRepository<OrganisationType> _organisationTypeRepository;

        public OrganisationTypeService(IUnitOfWorkFactory unitOfWork, ILogger logging, IRepository<OrganisationType> organisationTypeRepository)
        {
            _unitOfWork = unitOfWork;
            _logging = logging;
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
            
            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
                _organisationTypeRepository.Add(OrganisationType);
                work.Commit();
            }
            return OrganisationType;
        }

        public OrganisationType GetOrganisationTypeByName(string organisationTypeName)
        {
            return _organisationTypeRepository.FindAll().FirstOrDefault(ot => ot.Name == organisationTypeName);
        }

        #endregion
    }
}

