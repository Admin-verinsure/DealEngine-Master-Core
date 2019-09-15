using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;


namespace TechCertain.Services.Impl
{
    public class ClientAgreementMVTermService : IClientAgreementMVTermService
    {
        IUnitOfWorkFactory _unitOfWork;
        IRepository<ClientAgreementMVTerm> _clientAgreementMVTermRepository;

        public ClientAgreementMVTermService(IUnitOfWorkFactory unitOfWork, IRepository<ClientAgreementMVTerm> clientAgreementMVTermRepository)
        {
            _unitOfWork = unitOfWork;
            _clientAgreementMVTermRepository = clientAgreementMVTermRepository;
        }

        public bool AddAgreementMVTerm(User createdBy, string registration, string year, string make, string model, int termLimit, decimal excess, decimal premium, decimal fSL, decimal brokerageRate, decimal brokerage, string vehicleCategory, string fleetNumber, ClientAgreementTerm clientAgreementTerm, Vehicle vehicle, decimal burnerpremium)
        {
            if (string.IsNullOrWhiteSpace(year))
                throw new ArgumentNullException(nameof(year));
            if (string.IsNullOrWhiteSpace(make))
                throw new ArgumentNullException(nameof(make));
            //Carjam returns null
            //if (string.IsNullOrWhiteSpace(model))
            //    throw new ArgumentNullException(nameof(model));
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
            if (string.IsNullOrWhiteSpace(vehicleCategory.ToString()))
                throw new ArgumentNullException(nameof(vehicleCategory));
            if (clientAgreementTerm == null)
                throw new ArgumentNullException(nameof(clientAgreementTerm));
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
                ClientAgreementMVTerm clientAgreementMVTerm = new ClientAgreementMVTerm(createdBy, registration, year, make, model, termLimit, excess, premium, fSL, brokerageRate, brokerage, vehicleCategory, fleetNumber, clientAgreementTerm, vehicle, burnerpremium);
                _clientAgreementMVTermRepository.Add(clientAgreementMVTerm);
                work.Commit();
            }

            return true;
        }

        public IQueryable<ClientAgreementMVTerm> GetAllAgreementMVTermFor(ClientAgreementTerm clientAgreementTerm)
        {
            var mvterm = _clientAgreementMVTermRepository.FindAll().Where(cagmvt => cagmvt.ClientAgreementTerm == clientAgreementTerm &&
                                                                                    cagmvt.DateDeleted == null && cagmvt.TermCategory == "active");
            return mvterm;
        }

        public bool UpdateAgreementMVTerm(ClientAgreementMVTerm clientAgreementMVTerm)
        {
            _clientAgreementMVTermRepository.Add(clientAgreementMVTerm);
            return true;
        }

		public bool DeleteAgreementMVTerm (User deletedBy, ClientAgreementMVTerm clientAgreementMVTerm)
		{
			clientAgreementMVTerm.Delete (deletedBy);
			return UpdateAgreementMVTerm (clientAgreementMVTerm);
		}

        public IQueryable<ClientAgreementMVTerm> GetAgreementMVTermFor(ClientAgreementTerm clientAgreementTerm, Vehicle vehicle)
        {
            var mvterm = _clientAgreementMVTermRepository.FindAll().Where(cagmvt => cagmvt.ClientAgreementTerm == clientAgreementTerm &&
                                                                                    cagmvt.Vehicle == vehicle);
            return mvterm;
        }
    }
}
