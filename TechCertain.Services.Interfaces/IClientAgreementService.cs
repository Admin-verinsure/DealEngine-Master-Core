using System;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClientAgreementService
    {
        bool CreateClientAgreement(User createdBy, string insuredName, DateTime inceptionDate, DateTime expiryDate, decimal brokerage, decimal brokerFee, ClientInformationSheet clientInformationSheet);

        ClientAgreement GetAgreement(Guid clientAgreementId);

		ClientAgreement AcceptAgreement (ClientAgreement agreement, User acceptingUser);
        ClientAgreement GetAgreementbyReferenceNum(string reference);

    }
}
