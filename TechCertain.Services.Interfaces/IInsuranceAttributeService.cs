using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IInsuranceAttributeService
    {
        //void AddNew(string name);

       // IEnumerable<InsuranceAttribute> GetOrganisationTypes();

        InsuranceAttribute CreateNewInsuranceAttribute(User user, string insuranceAttribute);

        InsuranceAttribute GetInsuranceAttributeByName(string InsuranceAttribute);

       // bool UpdateOrganisationType(InsuranceAttribute insuranceAttribute);
    }
}
