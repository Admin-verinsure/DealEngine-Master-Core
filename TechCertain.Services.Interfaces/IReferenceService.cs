using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IReferenceService
    {
        Task<string> GetLatestReferenceId();
        Task CreateClientInformationReference(ClientInformationSheet ClientInformationSheet);
        Task CreateClientAgreementReference(string Reference, Guid ClientAgreementId);
        Task<bool> HasInformationId(Guid id);
        Task<bool> HasAgreementId(Guid id);
        Task<bool> HasReference(string Reference);
        Task CreateAsync(Reference Reference);
    }
}

