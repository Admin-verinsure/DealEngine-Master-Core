using System;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IBusinessContractService
    {
        Task<BusinessContract> GetBusinessContractById(Guid businessContractId);
        Task Update(BusinessContract businessContract);
    }
}
