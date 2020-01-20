using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IBusinessContractService
    {
        Task<BusinessContract> GetBusinessContractById(Guid businessContractId);
    }
}
