using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class ReferenceService : IReferenceService
	{
        IMapperSession<Reference> _referenceRepository;

		public ReferenceService(IMapperSession<Reference> referenceRepository)
        {
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

        public async void Update(Reference Reference)
        {
            _referenceRepository.AddAsync(Reference);
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

        public void CreateClientAgreementReference(string Reference, Guid ClientAgreementId)
        {
            if (ClientAgreementId == Guid.Empty)
                throw new ArgumentNullException(nameof(ClientAgreementId));

            if (!HasAgreementId(ClientAgreementId))
            {
                if (!HasReference(Reference))
                {
                    Reference = GetLatestReferenceId();
                }

                if (Reference == null)
                    throw new ArgumentNullException(nameof(Reference));

                Reference referenceObj = new Reference
                {
                    ClientAgreementId = ClientAgreementId,
                    ReferenceId = Reference,
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

        public bool HasReference(string Reference)
        {
            var referenceExist =_referenceRepository.FindAll().FirstOrDefault(m => m.ReferenceId == Reference);

            if(referenceExist != null)
                return true;

            return false;
        }
    }
}
