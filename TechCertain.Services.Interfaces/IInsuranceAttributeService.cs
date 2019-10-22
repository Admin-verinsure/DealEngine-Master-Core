using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IInsuranceAttributeService
    {
        //void AddNew(string name);

        // IEnumerable<InsuranceAttribute> GetOrganisationTypes();

        Task<InsuranceAttribute> CreateNewInsuranceAttribute(User user, string insuranceAttribute);

        Task<InsuranceAttribute> GetInsuranceAttributeByName(string InsuranceAttribute);

       // bool UpdateOrganisationType(InsuranceAttribute insuranceAttribute);
    }
}
