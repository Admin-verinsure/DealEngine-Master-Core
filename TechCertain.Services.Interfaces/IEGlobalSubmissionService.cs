using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IEGlobalSubmissionService
    {
        EGlobalSubmission CreateNewEGlobalSubmission(EGlobalSubmission eGlobalSubmission);

        bool DeleteEGlobalSubmission(User deletedBy, EGlobalSubmission eGlobalSubmission);

        IQueryable<EGlobalSubmission> GetAllEGlobalSubmissions();

        EGlobalSubmission GetEGlobalSubmission(Guid eGlobalSubmissionId);

        bool UpdateEGlobalSubmission(EGlobalSubmission eGlobalSubmission);

    }
}
