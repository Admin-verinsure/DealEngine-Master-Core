using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class EGlobalResponseService : IEGlobalResponseService
    {
        IMapperSession<EGlobalResponse> _eGlobalResponseRepository;

        public EGlobalResponseService(IMapperSession<EGlobalResponse> eGlobalResponseRepository)
        {
            _eGlobalResponseRepository = eGlobalResponseRepository;
        }

        public async Task<EGlobalResponse> CreateNewEGlobalResponse(EGlobalResponse eGlobalResponse)
        {
            await UpdateEGlobalResponse(eGlobalResponse);
            return eGlobalResponse;
        }

        public async Task<bool> DeleteEGlobalResponse(User deletedBy, EGlobalResponse eGlobalResponse)
        {
            eGlobalResponse.Delete(deletedBy);
            return await UpdateEGlobalResponse(eGlobalResponse);
        }

        public async Task<List<EGlobalResponse>> GetAllEGlobalResponses()
        {
            return await _eGlobalResponseRepository.FindAll().Where(egs => egs.DateDeleted != null).ToListAsync();
        }

        public async Task<EGlobalResponse> GetEGlobalResponse(Guid eGlobalResponseId)
        {
            EGlobalResponse eGlobalResponse = await _eGlobalResponseRepository.GetByIdAsync(eGlobalResponseId);
            if (eGlobalResponse != null)
                return eGlobalResponse;
            if (eGlobalResponse != null)
            {
                await UpdateEGlobalResponse(eGlobalResponse);
                return eGlobalResponse;
            }
            throw new Exception("EGlobalResponse with id [" + eGlobalResponseId + "] does not exist in the system");
        }

        public async Task<bool> UpdateEGlobalResponse(EGlobalResponse eGlobalResponse)
        {
            await _eGlobalResponseRepository.UpdateAsync(eGlobalResponse);
            return true;
        }


    }
}


