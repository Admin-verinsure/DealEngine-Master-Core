
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using System.Linq;
using TechCertain.Infrastructure.FluentNHibernate;

namespace TechCertain.Services.Impl
{
	public class InsuranceAttributeService : IInsuranceAttributeService
	{      
        IMapperSession<InsuranceAttribute> _InsuranceAttributeRepository;

        public InsuranceAttributeService(IMapperSession<InsuranceAttribute> insuranceAttributeRepository)
        {         
            _InsuranceAttributeRepository = insuranceAttributeRepository;
        }

        public InsuranceAttribute CreateNewInsuranceAttribute(User user, string insuranceAttributeName)
        {
            InsuranceAttribute insuranceAttribute = new InsuranceAttribute(user, insuranceAttributeName);
            _InsuranceAttributeRepository.AddAsync(insuranceAttribute);

            return insuranceAttribute;
        }
        #region IOrganisationTypeService implementation


        public InsuranceAttribute GetInsuranceAttributeByName(string InsuranceAttributeName)
        {
            return _InsuranceAttributeRepository.FindAll().FirstOrDefault(ot => ot.InsuranceAttributeName == InsuranceAttributeName);
        }

        #endregion
    }
}

