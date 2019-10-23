using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;


namespace TechCertain.Services.Impl
{
    public class ClientAgreementTermService : IClientAgreementTermService
    {
        IMapperSession<ClientAgreementTerm> _clientAgreementTermRepository;
        IMapperSession<ClientAgreement> _clientAgreementRepository;

        public ClientAgreementTermService(IMapperSession<ClientAgreementTerm> clientAgreementTermRepository, IMapperSession<ClientAgreement> clientAgreementRepository)
        {
            _clientAgreementTermRepository = clientAgreementTermRepository;
            _clientAgreementRepository = clientAgreementRepository;
        }

        public async Task AddAgreementTerm(User createdBy, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreement clientAgreement, string subTermType)
        {
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
            if (clientAgreement == null)
                throw new ArgumentNullException(nameof(clientAgreement));
            
		    ClientAgreementTerm clientAgreementTerm = new ClientAgreementTerm(createdBy, termLimit, excess, premium, fSL, brokerageRate, brokerage, clientAgreement, subTermType);
            clientAgreement.ClientAgreementTerms.Add(clientAgreementTerm);
            await _clientAgreementTermRepository.AddAsync(clientAgreementTerm);
            await _clientAgreementRepository.UpdateAsync(clientAgreement);

        }

        
        public async Task<List<ClientAgreementTerm>> GetAllAgreementTermFor(ClientAgreement clientAgreement)
        {
            return await _clientAgreementTermRepository.FindAll().Where(cagt => cagt.ClientAgreement == clientAgreement && 
                                                                              cagt.DateDeleted == null).ToListAsync();            
        }

        public async Task UpdateAgreementTerm(ClientAgreementTerm clientAgreementTerm)
        {
            await _clientAgreementTermRepository.AddAsync(clientAgreementTerm);            
        }

		public async Task DeleteAgreementTerm (User deletedBy, ClientAgreementTerm clientAgreementTerm)
		{
			clientAgreementTerm.Delete (deletedBy);
			await UpdateAgreementTerm (clientAgreementTerm);
		}

        public async Task<List<ClientAgreementTerm>> GetListAgreementTermFor(ClientAgreement clientAgreement)
        {
            return await _clientAgreementTermRepository.FindAll().Where(cagt => cagt.ClientAgreement == clientAgreement &&
                                                                              cagt.DateDeleted == null).ToListAsync();            
        }
    }
}
