
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using System.Linq;
using TechCertain.Domain.Interfaces;

namespace TechCertain.Services.Impl
{
	public class InsuranceAttributeService : IInsuranceAttributeService
	{

        IUnitOfWorkFactory _unitOfWork;
        ILogger _logging;
        IRepository<InsuranceAttribute> _InsuranceAttributeRepository;

        public InsuranceAttributeService(IUnitOfWorkFactory unitOfWork, ILogger logging, IRepository<InsuranceAttribute> insuranceAttributeRepository)
        {
            _unitOfWork = unitOfWork;
            _logging = logging;
            _InsuranceAttributeRepository = insuranceAttributeRepository;
        }

        public InsuranceAttribute CreateNewInsuranceAttribute(User user, string insuranceAttributeName)
        {
            InsuranceAttribute insuranceAttribute = new InsuranceAttribute(user, insuranceAttributeName);
            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
                _InsuranceAttributeRepository.Add(insuranceAttribute);
                work.Commit();
            }
           
            return insuranceAttribute;
        }
        #region IOrganisationTypeService implementation

        //public void AddNew (string name)
        //{
        //	throw new NotImplementedException ();
        //}

        //public IEnumerable<OrganisationType> GetOrganisationTypes ()
        //{
        //	throw new NotImplementedException ();
        //}

        //      public OrganisationType CreateNewOrganisationType(OrganisationType organisationType)
        //      {
        //          UpdateOrganisationType(organisationType);
        //          return organisationType;
        //      }

        //      public OrganisationType GetOrganisationTypeByName(string organisationTypeName)
        //      {
        //          return _organisationTypeRepository.FindAll().FirstOrDefault(ot => ot.Name == organisationTypeName);
        //      }

        //public bool UpdateOrganisationType(OrganisationType organisationType)
        //{
        //    _organisationTypeRepository.Add(organisationType);
        //    return true;
        //}

        public InsuranceAttribute GetInsuranceAttributeByName(string InsuranceAttributeName)
        {
            return _InsuranceAttributeRepository.FindAll().FirstOrDefault(ot => ot.InsuranceAttributeName == InsuranceAttributeName);
        }

        #endregion
    }
}

