using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;


namespace DealEngine.Services.Impl
{
    public class ClientAgreementBVTermService : IClientAgreementBVTermService
    {
        IMapperSession<ClientAgreementBVTerm> _clientAgreementBVTermRepository;

        public ClientAgreementBVTermService(IMapperSession<ClientAgreementBVTerm> clientAgreementBVTermRepository)
        {
            _clientAgreementBVTermRepository = clientAgreementBVTermRepository;
        }

        public async Task AddAgreementBVTerm(User createdBy, string boatName, int yearOfManufacture, string boatMake, string boatModel, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreementTerm clientAgreementTerm, Boat boat)
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
            await _clientAgreementBVTermRepository.AddAsync(clientAgreementBVTerm);
        }

        public async Task<List<ClientAgreementBVTerm>> GetAllAgreementBVTermFor(ClientAgreementTerm clientAgreementTerm)
        {
            return await _clientAgreementBVTermRepository.FindAll().Where(cagbvt => cagbvt.ClientAgreementTerm == clientAgreementTerm && cagbvt.DateDeleted == null && cagbvt.TermCategory == "active").ToListAsync();
        }

        public async Task UpdateAgreementBVTerm(ClientAgreementBVTerm clientAgreementBVTerm)
        {
            await _clientAgreementBVTermRepository.AddAsync(clientAgreementBVTerm);
        }

        public async Task DeleteAgreementBVTerm(User deletedBy, ClientAgreementBVTerm clientAgreementBVTerm)
        {
            clientAgreementBVTerm.Delete(deletedBy);
            await UpdateAgreementBVTerm(clientAgreementBVTerm);
        }

        public async Task<List<ClientAgreementBVTerm>> GetAgreementBVTermFor(ClientAgreementTerm clientAgreementTerm, Boat boat)
        {
            return await _clientAgreementBVTermRepository.FindAll().Where(cagbvt => cagbvt.ClientAgreementTerm == clientAgreementTerm &&
                                                                                    cagbvt.Boat == boat).ToListAsync();            
        }
    }
}
