using System;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;


namespace TechCertain.Services.Impl
{
    public class ClientAgreementService : IClientAgreementService
    {

        IUnitOfWorkFactory _unitOfWork;
        IRepository<ClientAgreement> _clientAgreementRepository;

        public ClientAgreementService(IUnitOfWorkFactory unitOfWork, IRepository<ClientAgreement> clientAgreementRepository)
        {
            _unitOfWork = unitOfWork;
            _clientAgreementRepository = clientAgreementRepository;
        }

        public bool CreateClientAgreement(User createdBy, string insuredName, DateTime inceptionDate, DateTime expiryDate, decimal brokerage, decimal brokerFee, ClientInformationSheet clientInformationSheet)
        {
            if (string.IsNullOrWhiteSpace(insuredName))
                throw new ArgumentNullException(nameof(insuredName));
            if (clientInformationSheet == null)
                throw new ArgumentNullException(nameof(clientInformationSheet));

            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
				ClientAgreement clientAgreement = new ClientAgreement (createdBy, insuredName, inceptionDate, expiryDate, brokerage, brokerFee, clientInformationSheet, null, clientInformationSheet.ReferenceId);
                clientInformationSheet.ClientAgreement = clientAgreement;
                _clientAgreementRepository.Add(clientAgreement);
                work.Commit();
            }

            return true;
        }

		public ClientAgreement GetAgreement (Guid clientAgreementId)
		{
			return _clientAgreementRepository.GetById (clientAgreementId);
		}

		public ClientAgreement AcceptAgreement (ClientAgreement agreement, User acceptingUser)
		{
			if (agreement == null)
				throw new ArgumentNullException (nameof (agreement));
			if (acceptingUser == null)
				throw new ArgumentNullException (nameof (acceptingUser));

			using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork ()) {
				agreement.ClientInformationSheet.Status = "Submitted";
				agreement.ClientInformationSheet.SubmitDate = DateTime.UtcNow;
				agreement.ClientInformationSheet.SubmittedBy = acceptingUser;
				// set accept flag here

				uow.Commit ();
			}

			return agreement;
		}
    }
}
