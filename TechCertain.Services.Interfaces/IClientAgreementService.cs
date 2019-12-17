using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientAgreementService
    {
        Task CreateClientAgreement(User createdBy, string insuredName, DateTime inceptionDate, DateTime expiryDate, decimal brokerage, decimal brokerFee, ClientInformationSheet clientInformationSheet);

        Task<ClientAgreement> GetAgreement(Guid clientAgreementId);

        Task<ClientAgreement> AcceptAgreement (ClientAgreement agreement, User acceptingUser);
        //ClientAgreement AcceptAgreement (ClientAgreement agreement, User acceptingUser);
        Task<ClientAgreement> GetAgreementbyReferenceNum(string reference);
        Task UpdateClientAgreement(ClientAgreement clientAgreement);

    }
}
