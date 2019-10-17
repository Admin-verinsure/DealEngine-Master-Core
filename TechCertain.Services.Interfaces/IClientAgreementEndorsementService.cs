using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
    {
        public interface IClientAgreementEndorsementService
        {
            void AddClientAgreementEndorsement(User createdBy, string name, string type, Product product, string value, int orderNumber, ClientAgreement clientAgreement);

            IQueryable<ClientAgreementEndorsement> GetAllClientAgreementEndorsementFor(ClientAgreement clientAgreement);

            ClientAgreementEndorsement GetClientAgreementEndorsementBy(Guid clientAgreementEndorsementId);
        }
    }
