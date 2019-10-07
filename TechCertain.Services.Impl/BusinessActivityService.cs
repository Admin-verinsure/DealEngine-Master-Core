using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class BusinessActivityService : IBusinessActivityService
    {
        IUnitOfWork _unitOfWork;
        IMapperSession<BusinessActivity> _businessActivityRepository;

        public BusinessActivityService(IUnitOfWork unitOfWork,
                                IMapperSession<BusinessActivity> businessActivityRepository)
        {
            _unitOfWork = unitOfWork;
            _businessActivityRepository = businessActivityRepository;
        }

        public void AttachClientProgrammeToActivities(Programme programme, BusinessActivity businessActivity)
        {
            businessActivity.Programme = programme;
            Update(businessActivity);
        }

        public void Update(BusinessActivity businessActivity)
        {
            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
            {
                _businessActivityRepository.Update(businessActivity);
                uow.Commit();
            }
        }

        public void CreateBusinessActivity(BusinessActivity businessActivity)
        {
            if (businessActivity.AnzsciCode != null || businessActivity.Description != null)
            {
                using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
                {
                    _businessActivityRepository.Add(businessActivity);
                    uow.Commit();
                }
            }
        }

        public IQueryable<BusinessActivity> GetBusinessActivities()
        {
            return _businessActivityRepository.FindAll().OrderBy(ba => ba.AnzsciCode);
        }

        public IQueryable<BusinessActivity> GetBusinessActivitiesByClassification(int classification)
        {
            return GetBusinessActivities().Where(ba => ba.Classification == classification);
        }

        public BusinessActivity GetBusinessActivity(Guid Id)
        {
            return _businessActivityRepository.GetById(Id);
        }

        public object GetBusinessActivitiesByClientProgramme(Guid programmeId)
        {
            return _businessActivityRepository.FindAll().Where(t => t.Programme.Id == programmeId);
        }
    }
}
