using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class ClientAgreementEndorsementService : IClientAgreementEndorsementService
    {
        IUnitOfWork _unitOfWork;
        IMapperSession<ClientAgreementEndorsement> _clientAgreementEndorsementRepository;

        public ClientAgreementEndorsementService(IUnitOfWork unitOfWork, IMapperSession<ClientAgreementEndorsement> clientAgreementEndorsementRepository)
        {
            _unitOfWork = unitOfWork;
            _clientAgreementEndorsementRepository = clientAgreementEndorsementRepository;
        }

        public bool AddClientAgreementEndorsement(User createdBy, string name, string type, Product product, string value, int orderNumber, ClientAgreement clientAgreement)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrWhiteSpace(orderNumber.ToString()))
                throw new ArgumentNullException(nameof(orderNumber));
            if (clientAgreement == null)
                throw new ArgumentNullException(nameof(clientAgreement));

            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
                ClientAgreementEndorsement clientAgreementEndorsement = new ClientAgreementEndorsement(createdBy, name, type, product, value, orderNumber, clientAgreement);
                clientAgreement.ClientAgreementEndorsements.Add(clientAgreementEndorsement);
                work.Commit();
            }

            return true;
        }


        public IQueryable<ClientAgreementEndorsement> GetAllClientAgreementEndorsementFor(ClientAgreement clientAgreement)
        {
            var clientAgreementEndorsement = _clientAgreementEndorsementRepository.FindAll().Where(cagt => cagt.ClientAgreement == clientAgreement);
            return clientAgreementEndorsement;
        }

        public ClientAgreementEndorsement GetClientAgreementEndorsementBy(Guid clientAgreementEndorsementId)
        {
            return _clientAgreementEndorsementRepository.GetById(clientAgreementEndorsementId);
        }
    }
}
