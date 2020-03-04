using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;

namespace DealEngine.Services.Impl
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

