using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;


namespace TechCertain.Services.Impl
{
    public class ClientAgreementBVTermService : IClientAgreementBVTermService
    {
        IUnitOfWorkFactory _unitOfWork;
        IRepository<ClientAgreementBVTerm> _clientAgreementBVTermRepository;

        public ClientAgreementBVTermService(IUnitOfWorkFactory unitOfWork, IRepository<ClientAgreementBVTerm> clientAgreementBVTermRepository)
        {
            _unitOfWork = unitOfWork;
            _clientAgreementBVTermRepository = clientAgreementBVTermRepository;
        }

        public bool AddAgreementBVTerm(User createdBy, string boatName, int yearOfManufacture, string boatMake, string boatModel, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, ClientAgreementTerm clientAgreementTerm, Boat boat)
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

            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
                ClientAgreementBVTerm clientAgreementBVTerm = new ClientAgreementBVTerm(createdBy, clientAgreementTerm, boat, boatName, yearOfManufacture, boatMake, boatModel, termLimit, excess, premium, fSL, brokerageRate, brokerage);
                _clientAgreementBVTermRepository.Add(clientAgreementBVTerm);
                work.Commit();
            }

            return true;
        }

        public IQueryable<ClientAgreementBVTerm> GetAllAgreementBVTermFor(ClientAgreementTerm clientAgreementTerm)
        {
            var bvterm = _clientAgreementBVTermRepository.FindAll().Where(cagbvt => cagbvt.ClientAgreementTerm == clientAgreementTerm &&
                                                                                    cagbvt.DateDeleted == null && cagbvt.TermCategory == "active");
            return bvterm;
        }

        public bool UpdateAgreementBVTerm(ClientAgreementBVTerm clientAgreementBVTerm)
        {
            _clientAgreementBVTermRepository.Add(clientAgreementBVTerm);
            return true;
        }

        public bool DeleteAgreementBVTerm(User deletedBy, ClientAgreementBVTerm clientAgreementBVTerm)
        {
            clientAgreementBVTerm.Delete(deletedBy);
            return UpdateAgreementBVTerm(clientAgreementBVTerm);
        }

        public IQueryable<ClientAgreementBVTerm> GetAgreementBVTermFor(ClientAgreementTerm clientAgreementTerm, Boat boat)
        {
            var bvterm = _clientAgreementBVTermRepository.FindAll().Where(cagbvt => cagbvt.ClientAgreementTerm == clientAgreementTerm &&
                                                                                    cagbvt.Boat == boat);
            return bvterm;
        }
    }
}
