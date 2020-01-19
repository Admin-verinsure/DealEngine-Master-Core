
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using System.Linq;
using TechCertain.Infrastructure.FluentNHibernate;
using System.Threading.Tasks;
using NHibernate.Linq;
using System.Collections.Generic;

namespace TechCertain.Services.Impl
{
	public class InsuranceAttributeService : IInsuranceAttributeService
	{      
        IMapperSession<InsuranceAttribute> _InsuranceAttributeRepository;

        public InsuranceAttributeService(IMapperSession<InsuranceAttribute> insuranceAttributeRepository)
        {         
            _InsuranceAttributeRepository = insuranceAttributeRepository;
        }

        public async Task<InsuranceAttribute> CreateNewInsuranceAttribute(User user, string insuranceAttributeName)
        {
            InsuranceAttribute insuranceAttribute = new InsuranceAttribute(user, insuranceAttributeName);
            await _InsuranceAttributeRepository.AddAsync(insuranceAttribute);

            return insuranceAttribute;
        }
        #region IOrganisationTypeService implementation


        public async Task<InsuranceAttribute> GetInsuranceAttributeByName(string InsuranceAttributeName)
        {
            return await _InsuranceAttributeRepository.FindAll().FirstOrDefaultAsync(ot => ot.InsuranceAttributeName == InsuranceAttributeName);
        }

        public async Task<List<InsuranceAttribute>> GetInsuranceAttributes()
        {
            return await _InsuranceAttributeRepository.FindAll().ToListAsync();
        }

        #endregion
    }
}

