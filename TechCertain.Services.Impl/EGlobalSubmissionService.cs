using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class EGlobalSubmissionService : IEGlobalSubmissionService
    {
        IUnitOfWork _unitOfWork;
        IMapperSession<EGlobalSubmission> _eGlobalSubmissionRepository;

        public EGlobalSubmissionService(IUnitOfWork unitOfWork, IMapperSession<EGlobalSubmission> eGlobalSubmissionRepository)
        {
            _unitOfWork = unitOfWork;
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
            EGlobalSubmission eGlobalSubmission = _eGlobalSubmissionRepository.GetById(eGlobalSubmissionId);
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
            _eGlobalSubmissionRepository.Add(eGlobalSubmission);
            return true;
        }


    }
}

