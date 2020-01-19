using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IInsuranceAttributeService
    {
        Task<List<InsuranceAttribute>> GetInsuranceAttributes();
        Task<InsuranceAttribute> CreateNewInsuranceAttribute(User user, string insuranceAttribute);
        Task<InsuranceAttribute> GetInsuranceAttributeByName(string InsuranceAttribute);       
    }
}
