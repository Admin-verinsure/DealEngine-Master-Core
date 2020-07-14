
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using System.Linq;
using DealEngine.Infrastructure.FluentNHibernate;
using System.Threading.Tasks;
using NHibernate.Linq;
using System.Collections.Generic;

namespace DealEngine.Services.Impl
{
	public class InsuranceAttributeService : IInsuranceAttributeService
	{      
        IMapperSession<InsuranceAttribute> _InsuranceAttributeRepository;

        public InsuranceAttributeService(IMapperSession<InsuranceAttribute> insuranceAttributeRepository)
        {         
            _InsuranceAttributeRepository = insuranceAttributeRepository;
        }

        public async Task<InsuranceAttribute> CreateNewInsuranceAttribute(User user, string Name)
        {
            InsuranceAttribute insuranceAttribute = new InsuranceAttribute(user, Name);
            await _InsuranceAttributeRepository.AddAsync(insuranceAttribute);

            return insuranceAttribute;
        }
        #region IOrganisationTypeService implementation


        public async Task<InsuranceAttribute> GetInsuranceAttributeByName(string Name)
        {            
            var attribute = await _InsuranceAttributeRepository.FindAll().FirstOrDefaultAsync(ot => ot.Name == Name);
            if(attribute == null)
            {
                attribute = await CreateNewInsuranceAttribute(null, Name);
            }
            return attribute;
        }

        public async Task<List<InsuranceAttribute>> GetInsuranceAttributes()
        {
            return await _InsuranceAttributeRepository.FindAll().ToListAsync();
        }

        #endregion
    }
}

