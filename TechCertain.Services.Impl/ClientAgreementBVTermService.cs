using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;


namespace TechCertain.Services.Impl
{
    public class ClientAgreementBVTermService : IClientAgreementBVTermService
    {
        IMapperSession<ClientAgreementBVTerm> _clientAgreementBVTermRepository;

        public ClientAgreementBVTermService(IMapperSession<ClientAgreementBVTerm> clientAgreementBVTermRepository)
        {
            _clientAgreementBVTermRepository = clientAgreementBVTermRepository;
        }

        public void AddAgreementBVTerm(User createdBy, string boatName, int yearOfManufacture, string boatMake, string boatModel, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreementTerm clientAgreementTerm, Boat boat)
        {
            if (string.IsNullOrWhiteSpace(boatName))
                throw new ArgumentNullException(nameof(boatName));           
            if (string.IsNullOrWhiteSpace(termLimit.ToString()))
                throw new ArgumentNullException(nameof(termLimit));
            if (string.IsNullOrWhiteSpace(excess.ToString()))
                throw new ArgumentNullException(nameof(excess));
            if (string.IsNullOrWhiteSpace(premium.ToString()))
                throw new ArgumentNullException(nameof(premium));
            if (string.IsNullOrWhiteSpace(fSL.ToString()))
                throw new ArgumentNullException(nameof(fSL));
            if (string.IsNullOrWhiteSpace(brokerageRate.ToString()))
                throw new ArgumentNullException(nameof(brokerageRate));
            if (string.IsNullOrWhiteSpace(brokerage.ToString()))
                throw new ArgumentNullException(nameof(brokerage));
            if (clientAgreementTerm == null)
                throw new ArgumentNullException(nameof(clientAgreementTerm));
            if (boat == null)
                throw new ArgumentNullException(nameof(boat));

            ClientAgreementBVTerm clientAgreementBVTerm = new ClientAgreementBVTerm(createdBy, clientAgreementTerm, boat, boatName, yearOfManufacture, boatMake, boatModel, termLimit, excess, premium, fSL, brokerageRate, brokerage);
            _clientAgreementBVTermRepository.AddAsync(clientAgreementBVTerm);
        }

        public IQueryable<ClientAgreementBVTerm> GetAllAgreementBVTermFor(ClientAgreementTerm clientAgreementTerm)
        {
            var bvterm = _clientAgreementBVTermRepository.FindAll().Where(cagbvt => cagbvt.ClientAgreementTerm == clientAgreementTerm &&
                                                                                    cagbvt.DateDeleted == null && cagbvt.TermCategory == "active");
            return bvterm;
        }

        public void UpdateAgreementBVTerm(ClientAgreementBVTerm clientAgreementBVTerm)
        {
            _clientAgreementBVTermRepository.AddAsync(clientAgreementBVTerm);
        }

        public void DeleteAgreementBVTerm(User deletedBy, ClientAgreementBVTerm clientAgreementBVTerm)
        {
            clientAgreementBVTerm.Delete(deletedBy);
            UpdateAgreementBVTerm(clientAgreementBVTerm);
        }

        public IQueryable<ClientAgreementBVTerm> GetAgreementBVTermFor(ClientAgreementTerm clientAgreementTerm, Boat boat)
        {
            var bvterm = _clientAgreementBVTermRepository.FindAll().Where(cagbvt => cagbvt.ClientAgreementTerm == clientAgreementTerm &&
                                                                                    cagbvt.Boat == boat);
            return bvterm;
        }
    }
}
