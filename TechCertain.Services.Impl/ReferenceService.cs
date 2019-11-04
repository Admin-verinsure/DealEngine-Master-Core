using NHibernate.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<string> GetLatestReferenceId()
        {
            var reference = await _referenceRepository.FindAll().Where(r => r.DateDeleted == null).OrderByDescending(r => r.ReferenceId).FirstOrDefaultAsync();
           
            if (reference == null)
            {
                return (1000000).ToString();
            }

            int.TryParse(reference.ReferenceId, out int nextReference);            
            return (nextReference + 1).ToString();
        }

        public async Task CreateAsync(Reference Reference)
        {
            await _referenceRepository.AddAsync(Reference);
        }

        public async Task CreateClientInformationReference(ClientInformationSheet ClientInformationSheet)
        {
            if (ClientInformationSheet == null)
                throw new ArgumentNullException(nameof(ClientInformationSheet));

            var hasInfoSheet = await HasInformationId(ClientInformationSheet.Id);
            if (!hasInfoSheet)
            {

                if (ClientInformationSheet.ReferenceId == null)
                    throw new ArgumentNullException(nameof(ClientInformationSheet.ReferenceId));

                Reference referenceObj = new Reference
                {
                    ClientInformationSheetId = ClientInformationSheet.Id,
                    ReferenceId = ClientInformationSheet.ReferenceId,
                };

                await CreateAsync(referenceObj);
            }
        }

        public async Task CreateClientAgreementReference(string Reference, Guid ClientAgreementId)
        {
            if (ClientAgreementId == Guid.Empty)
                throw new ArgumentNullException(nameof(ClientAgreementId));

            var hasAgreement = await HasAgreementId(ClientAgreementId);
            if (!hasAgreement)
            {
                var hasReference = await HasReference(Reference);
                if (!hasReference)
                {
                    Reference = await GetLatestReferenceId();
                }

                if (Reference == null)
                    throw new ArgumentNullException(nameof(Reference));

                Reference referenceObj = new Reference
                {
                    ClientAgreementId = ClientAgreementId,
                    ReferenceId = Reference,
                };

                await CreateAsync(referenceObj);
            }
              
        }

        public async Task<bool> HasInformationId(Guid id)
        {
            var referenceExist = await _referenceRepository.FindAll().FirstOrDefaultAsync(m => m.ClientInformationSheetId == id);

            if (referenceExist != null)
                return true;

            return false;
        }

        public async Task<bool> HasAgreementId(Guid id)
        {
            var referenceExist = await _referenceRepository.FindAll().FirstOrDefaultAsync(m => m.ClientAgreementId == id);

            if (referenceExist != null)
                return true;

            return false;
        }

        public async Task<bool> HasReference(string Reference)
        {
            var referenceExist = await _referenceRepository.FindAll().FirstOrDefaultAsync(m => m.ReferenceId == Reference);

            if(referenceExist != null)
                return true;

            return false;
        }
    }
}
