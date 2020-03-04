using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IEGlobalResponseService
    {
        Task<EGlobalResponse> CreateNewEGlobalResponse(EGlobalResponse eGlobalResponse);

        Task<bool> DeleteEGlobalResponse(User deletedBy, EGlobalResponse eGlobalResponse);

        Task<List<EGlobalResponse>> GetAllEGlobalResponses();

        Task<EGlobalResponse> GetEGlobalResponse(Guid eGlobalResponseId);

        Task<bool> UpdateEGlobalResponse(EGlobalResponse eGlobalResponse);

    }
}
