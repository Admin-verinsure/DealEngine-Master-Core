using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class EGlobalSubmissionService : IEGlobalSubmissionService
    {
        IMapperSession<EGlobalSubmission> _eGlobalSubmissionRepository;

        public EGlobalSubmissionService(IMapperSession<EGlobalSubmission> eGlobalSubmissionRepository)
        {
            _eGlobalSubmissionRepository = eGlobalSubmissionRepository;
        }

        public EGlobalSubmission CreateNewEGlobalSubmission(EGlobalSubmission eGlobalSubmission)
        {
            UpdateEGlobalSubmission(eGlobalSubmission);
            return eGlobalSubmission;
        }

        public bool DeleteEGlobalSubmission(User deletedBy, EGlobalSubmission eGlobalSubmission)
        {
            eGlobalSubmission.Delete(deletedBy);
            return UpdateEGlobalSubmission(eGlobalSubmission);
        }

        public IQueryable<EGlobalSubmission> GetAllEGlobalSubmissions()
        {
            return _eGlobalSubmissionRepository.FindAll().Where(egs => egs.DateDeleted != null);
        }

        public EGlobalSubmission GetEGlobalSubmission(Guid eGlobalSubmissionId)
        {
            EGlobalSubmission eGlobalSubmission = _eGlobalSubmissionRepository.GetByIdAsync(eGlobalSubmissionId).Result;
            if (eGlobalSubmission != null)
                return eGlobalSubmission;
            if (eGlobalSubmission != null)
            {
                UpdateEGlobalSubmission(eGlobalSubmission);
                return eGlobalSubmission;
            }
            throw new Exception("EGlobalSubmission with id [" + eGlobalSubmissionId + "] does not exist in the system");
        }

        public bool UpdateEGlobalSubmission(EGlobalSubmission eGlobalSubmission)
        {
            _eGlobalSubmissionRepository.UpdateAsync(eGlobalSubmission);
            return true;
        }


    }
}

