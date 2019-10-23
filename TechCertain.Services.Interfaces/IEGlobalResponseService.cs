using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IEGlobalResponseService
    {
        EGlobalResponse CreateNewEGlobalResponse(EGlobalResponse eGlobalResponse);

        bool DeleteEGlobalResponse(User deletedBy, EGlobalResponse eGlobalResponse);

        IQueryable<EGlobalResponse> GetAllEGlobalResponses();

        EGlobalResponse GetEGlobalResponse(Guid eGlobalResponseId);

        bool UpdateEGlobalResponse(EGlobalResponse eGlobalResponse);

    }
}
