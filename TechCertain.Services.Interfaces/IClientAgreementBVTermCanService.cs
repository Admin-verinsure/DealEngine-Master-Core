using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientAgreementBVTermCanService
    {
        Task AddAgreementBVTermCan(User createdBy, string boatNameCan, int yearOfManufactureCan, string boatMakeCan, string boatModelCan, int termLimitCan, decimal excessCan, decimal premiumCan, 
            decimal fSLCan, decimal brokerageRateCan, decimal brokerageCan, ClientAgreementTermCancel clientAgreementTermCan, Boat boatCan);

        Task<List<ClientAgreementBVTermCancel>> GetAllAgreementBVTermCanFor(ClientAgreementTermCancel clientAgreementTermCan);

        Task UpdateAgreementBVTermCan(ClientAgreementBVTermCancel clientAgreementBVTermCan);

        Task DeleteAgreementBVTermCan(User deletedBy, ClientAgreementBVTermCancel clientAgreementBVTermCan);

        Task<List<ClientAgreementBVTermCancel>> GetAgreementBVTermCanFor(ClientAgreementTermCancel clientAgreementTermCan, Boat boatCan);
    }
}