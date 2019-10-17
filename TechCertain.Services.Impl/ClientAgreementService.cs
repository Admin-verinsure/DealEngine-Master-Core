using System;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;


namespace TechCertain.Services.Impl
{
    public class ClientAgreementService : IClientAgreementService
    {
        IMapperSession<ClientAgreement> _clientAgreementRepository;
        IMapperSession<ClientInformationSheet> _clientInformationSheetRepository;

        public ClientAgreementService(IMapperSession<ClientAgreement> clientAgreementRepository, IMapperSession<ClientInformationSheet> clientInformationSheetRepository)
        {
            _clientAgreementRepository = clientAgreementRepository;
            _clientInformationSheetRepository = clientInformationSheetRepository;
        }

        public void CreateClientAgreement(User createdBy, string insuredName, DateTime inceptionDate, DateTime expiryDate, decimal brokerage, decimal brokerFee, ClientInformationSheet clientInformationSheet)
        {
            if (string.IsNullOrWhiteSpace(insuredName))
                throw new ArgumentNullException(nameof(insuredName));
            if (clientInformationSheet == null)
                throw new ArgumentNullException(nameof(clientInformationSheet));

            ClientAgreement clientAgreement = new ClientAgreement (createdBy, insuredName, inceptionDate, expiryDate, brokerage, brokerFee, clientInformationSheet, null, clientInformationSheet.ReferenceId);
            clientInformationSheet.ClientAgreement = clientAgreement;
            _clientInformationSheetRepository.UpdateAsync(clientInformationSheet);
            _clientAgreementRepository.AddAsync(clientAgreement);
        }

		public ClientAgreement GetAgreement (Guid clientAgreementId)
		{
			return _clientAgreementRepository.GetByIdAsync(clientAgreementId).Result;
		}

		public ClientAgreement AcceptAgreement (ClientAgreement agreement, User acceptingUser)
		{
			if (agreement == null)
				throw new ArgumentNullException (nameof (agreement));
			if (acceptingUser == null)
				throw new ArgumentNullException (nameof (acceptingUser));

		    agreement.ClientInformationSheet.Status = "Submitted";
			agreement.ClientInformationSheet.SubmitDate = DateTime.UtcNow;
			agreement.ClientInformationSheet.SubmittedBy = acceptingUser;
			
            _clientAgreementRepository.UpdateAsync(agreement);
            return agreement;
		}
    }
}
