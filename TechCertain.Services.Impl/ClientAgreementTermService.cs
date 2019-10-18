using System;
using System.Collections.Generic;
using System.Linq;
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

        public void AddAgreementTerm(User createdBy, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreement clientAgreement, string subTermType)
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
            _clientAgreementTermRepository.AddAsync(clientAgreementTerm);
            _clientAgreementRepository.UpdateAsync(clientAgreement);

        }

        
        public IQueryable<ClientAgreementTerm> GetAllAgreementTermFor(ClientAgreement clientAgreement)
        {
            var term = _clientAgreementTermRepository.FindAll().Where(cagt => cagt.ClientAgreement == clientAgreement && 
                                                                              cagt.DateDeleted == null);
            return term;
        }

        public void UpdateAgreementTerm(ClientAgreementTerm clientAgreementTerm)
        {
            _clientAgreementTermRepository.AddAsync(clientAgreementTerm);            
        }

		public void DeleteAgreementTerm (User deletedBy, ClientAgreementTerm clientAgreementTerm)
		{
			clientAgreementTerm.Delete (deletedBy);
			UpdateAgreementTerm (clientAgreementTerm);
		}

        public IList<ClientAgreementTerm> GetListAgreementTermFor(ClientAgreement clientAgreement)
        {
            var term = _clientAgreementTermRepository.FindAll().Where(cagt => cagt.ClientAgreement == clientAgreement &&
                                                                              cagt.DateDeleted == null).ToList();
            return term;
        }
    }
}
