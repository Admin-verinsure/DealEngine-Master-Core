using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IInsuranceAttributeService
    {
        Task<List<InsuranceAttribute>> GetInsuranceAttributes();
        Task<InsuranceAttribute> CreateNewInsuranceAttribute(User user, string insuranceAttribute);
        Task<InsuranceAttribute> GetInsuranceAttributeByName(string InsuranceAttribute);       
    }
}
