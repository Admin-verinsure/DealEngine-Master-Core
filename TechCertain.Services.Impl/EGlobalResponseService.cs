using System;
using System.Linq;
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

        public EGlobalResponse CreateNewEGlobalResponse(EGlobalResponse eGlobalResponse)
        {
            UpdateEGlobalResponse(eGlobalResponse);
            return eGlobalResponse;
        }

        public bool DeleteEGlobalResponse(User deletedBy, EGlobalResponse eGlobalResponse)
        {
            eGlobalResponse.Delete(deletedBy);
            return UpdateEGlobalResponse(eGlobalResponse);
        }

        public IQueryable<EGlobalResponse> GetAllEGlobalResponses()
        {
            return _eGlobalResponseRepository.FindAll().Where(egs => egs.DateDeleted != null);
        }

        public EGlobalResponse GetEGlobalResponse(Guid eGlobalResponseId)
        {
            EGlobalResponse eGlobalResponse = _eGlobalResponseRepository.GetByIdAsync(eGlobalResponseId).Result;
            if (eGlobalResponse != null)
                return eGlobalResponse;
            if (eGlobalResponse != null)
            {
                UpdateEGlobalResponse(eGlobalResponse);
                return eGlobalResponse;
            }
            throw new Exception("EGlobalResponse with id [" + eGlobalResponseId + "] does not exist in the system");
        }

        public bool UpdateEGlobalResponse(EGlobalResponse eGlobalResponse)
        {
            _eGlobalResponseRepository.UpdateAsync(eGlobalResponse);
            return true;
        }


    }
}


