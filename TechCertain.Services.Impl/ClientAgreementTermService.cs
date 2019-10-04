using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;


namespace TechCertain.Services.Impl
{
    public class ClientAgreementTermService : IClientAgreementTermService
    {
        IUnitOfWork _unitOfWork;
        IMapperSession<ClientAgreementTerm> _clientAgreementTermRepository;

        public ClientAgreementTermService(IUnitOfWork unitOfWork, IMapperSession<ClientAgreementTerm> clientAgreementTermRepository)
        {
            _unitOfWork = unitOfWork;
            _clientAgreementTermRepository = clientAgreementTermRepository;
        }

        public bool AddAgreementTerm(User createdBy, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreement clientAgreement, string subTermType)
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
            
            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
				ClientAgreementTerm clientAgreementTerm = new ClientAgreementTerm(createdBy, termLimit, excess, premium, fSL, brokerageRate, brokerage, clientAgreement, subTermType);
                clientAgreement.ClientAgreementTerms.Add(clientAgreementTerm);
                work.Commit();
            }

            return true;
        }

        
        public IQueryable<ClientAgreementTerm> GetAllAgreementTermFor(ClientAgreement clientAgreement)
        {
            var term = _clientAgreementTermRepository.FindAll().Where(cagt => cagt.ClientAgreement == clientAgreement && 
                                                                              cagt.DateDeleted == null);
            return term;
        }

        public bool UpdateAgreementTerm(ClientAgreementTerm clientAgreementTerm)
        {
            _clientAgreementTermRepository.Add(clientAgreementTerm);
            return true;
        }

		public bool DeleteAgreementTerm (User deletedBy, ClientAgreementTerm clientAgreementTerm)
		{
			clientAgreementTerm.Delete (deletedBy);
			return UpdateAgreementTerm (clientAgreementTerm);
		}

        public IList<ClientAgreementTerm> GetListAgreementTermFor(ClientAgreement clientAgreement)
        {
            var term = _clientAgreementTermRepository.FindAll().Where(cagt => cagt.ClientAgreement == clientAgreement &&
                                                                              cagt.DateDeleted == null).ToList();
            return term;
        }
    }
}
