using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientAgreementTermCanService
    {

        Task AddAgreementTermCan(User createdBy, int termLimitCan, decimal excessCan, decimal premiumCan, decimal fSLCan, decimal brokerageRateCan, decimal brokerageCan, 
            ClientAgreement clientAgreementCan, string subTermTypeCan);

        Task<List<ClientAgreementTermCancel>> GetAllAgreementTermCanFor(ClientAgreement clientAgreementCan);

        Task<List<ClientAgreementTermCancel>> GetListAgreementTermCanFor(ClientAgreement clientAgreementCan);

        Task UpdateAgreementTermCan(ClientAgreementTermCancel clientAgreementTermCan);

        Task DeleteAgreementTermCan(User deletedBy, ClientAgreementTermCancel clientAgreementTermCan);
        Task<List<ClientAgreementTermCancel>> GetAllClientAgreementTermCan();
    }
}
