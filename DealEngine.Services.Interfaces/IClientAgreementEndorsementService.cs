using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
    {
        public interface IClientAgreementEndorsementService
        {
            Task AddClientAgreementEndorsement(User createdBy, string name, string type, Product product, string value, int orderNumber, ClientAgreement clientAgreement);

            Task<List<ClientAgreementEndorsement>> GetAllClientAgreementEndorsementFor(ClientAgreement clientAgreement);

            Task<ClientAgreementEndorsement> GetClientAgreementEndorsementBy(Guid clientAgreementEndorsementId);
        }
    }
