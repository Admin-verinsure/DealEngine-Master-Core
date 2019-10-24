using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientAgreementBVTermService
    {
        Task AddAgreementBVTerm(User createdBy, string boatName, int yearOfManufacture, string boatMake, string boatModel, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreementTerm clientAgreementTerm, Boat boat);

        Task<List<ClientAgreementBVTerm>> GetAllAgreementBVTermFor(ClientAgreementTerm clientAgreementTerm);

        Task UpdateAgreementBVTerm(ClientAgreementBVTerm clientAgreementBVTerm);

        Task DeleteAgreementBVTerm(User deletedBy, ClientAgreementBVTerm clientAgreementBVTerm);

        Task<List<ClientAgreementBVTerm>> GetAgreementBVTermFor(ClientAgreementTerm clientAgreementTerm, Boat boat);
    }
}
