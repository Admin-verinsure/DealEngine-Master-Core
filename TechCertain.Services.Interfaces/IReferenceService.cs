using System;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IReferenceService
    {
        string GetLatestReferenceId();
        void CreateClientInformationReference(ClientInformationSheet ClientInformationSheet);
        void CreateClientAgreementReference(string Reference, Guid ClientAgreementId);
        bool HasInformationId(Guid id);
        bool HasAgreementId(Guid id);
        bool HasReference(string Reference);
        void Update(Reference Reference);
    }
}

