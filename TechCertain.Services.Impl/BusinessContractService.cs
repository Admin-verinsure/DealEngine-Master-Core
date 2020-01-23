using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;

namespace TechCertain.Services.Impl
{
    public class BusinessContractService : IBusinessContractService
    {
        IMapperSession<BusinessContract> _businessContractRepository;

        public BusinessContractService(IMapperSession<BusinessContract> businessContractRepository)
        {
            _businessContractRepository = businessContractRepository;
        }

        public async Task<BusinessContract> GetBusinessContractById(Guid businessContractId)
        {
            return await _businessContractRepository.GetByIdAsync(businessContractId);
        }
    }
}

