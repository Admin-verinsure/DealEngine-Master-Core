using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientAgreementMVTermService
    {
        Task AddAgreementMVTerm(User createdBy, string registration, string year, string make, string model, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, string vehicleCategory, string fleetNumber, ClientAgreementTerm clientAgreementTerm, Vehicle vehicle, decimal burnerpremium);

        Task<List<ClientAgreementMVTerm>> GetAllAgreementMVTermFor(ClientAgreementTerm clientAgreementTerm);

        Task UpdateAgreementMVTerm(ClientAgreementMVTerm clientAgreementMVTerm);

        Task DeleteAgreementMVTerm(User deletedBy, ClientAgreementMVTerm clientAgreementMVTerm);

        Task<List<ClientAgreementMVTerm>> GetAgreementMVTermFor(ClientAgreementTerm clientAgreementTerm, Vehicle vehicle);
    }
}
