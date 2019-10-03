using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class ReferenceService : IReferenceService
	{
		IUnitOfWork _unitOfWork;
        IMapperSession<Reference> _referenceRepository;

		public ReferenceService(IUnitOfWork unitOfWork, IMapperSession<Reference> referenceRepository)
        {
			_unitOfWork = unitOfWork;
            _referenceRepository = referenceRepository;
        }

        public string GetLatestReferenceId()
        {
            Reference reference = _referenceRepository.FindAll().Where(r => r.DateDeleted == null).OrderByDescending(r => r.ReferenceId).FirstOrDefault();
            if (reference == null)
            {
                return (1000000).ToString();
            }

            int.TryParse(reference.ReferenceId, out int nextReference);            
            return (nextReference + 1).ToString();
        }

        public void Update(Reference reference)
        {
            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
                _referenceRepository.Add(reference);
                work.Commit();
            }
        }

        public void CreateClientInformationReference(ClientInformationSheet ClientInformationSheet)
        {
            if (ClientInformationSheet == null)
                throw new ArgumentNullException(nameof(ClientInformationSheet));

            if (!HasInformationId(ClientInformationSheet.Id))
            {

                if (ClientInformationSheet.ReferenceId == null)
                    throw new ArgumentNullException(nameof(ClientInformationSheet.ReferenceId));

                Reference referenceObj = new Reference
                {
                    ClientInformationSheetId = ClientInformationSheet.Id,
                    ReferenceId = ClientInformationSheet.ReferenceId,
                };

                Update(referenceObj);
            }           
        }

        public void CreateClientAgreementReference(string reference, Guid ClientAgreementId)
        {
            if (ClientAgreementId == Guid.Empty)
                throw new ArgumentNullException(nameof(ClientAgreementId));

            if (!HasAgreementId(ClientAgreementId))
            {
                if (!HasReference(reference))
                {
                    reference = GetLatestReferenceId();
                }

                if (reference == null)
                    throw new ArgumentNullException(nameof(reference));

                Reference referenceObj = new Reference
                {
                    ClientAgreementId = ClientAgreementId,
                    ReferenceId = reference,
                };

                Update(referenceObj);
            }
              
        }

        public bool HasInformationId(Guid id)
        {
            var referenceExist = _referenceRepository.FindAll().FirstOrDefault(m => m.ClientInformationSheetId == id);

            if (referenceExist != null)
                return true;

            return false;
        }

        public bool HasAgreementId(Guid id)
        {
            var referenceExist = _referenceRepository.FindAll().FirstOrDefault(m => m.ClientAgreementId == id);

            if (referenceExist != null)
                return true;

            return false;
        }

        public bool HasReference(string reference)
        {
            var referenceExist =_referenceRepository.FindAll().FirstOrDefault(m => m.ReferenceId == reference);

            if(referenceExist != null)
                return true;

            return false;
        }
    }
}
