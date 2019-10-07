using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class MerchantService : IMerchantService
    {
        IUnitOfWork _unitOfWork;
        IMapperSession<Merchant> _merchantRepository;

        public MerchantService(IUnitOfWork unitOfWork, IMapperSession<Merchant> merchantRepository)
        {
            _unitOfWork = unitOfWork;
            _merchantRepository = merchantRepository;
        }

        public bool AddNewMerchant(User createdBy, string merchantUserName, string merchantPassword, string merchantKey, string merchantReference)
        {
            if (string.IsNullOrWhiteSpace(merchantUserName))
                throw new ArgumentNullException(nameof(merchantUserName));
            if (string.IsNullOrWhiteSpace(merchantKey))
                throw new ArgumentNullException(nameof(merchantKey));

            using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
            {
                _merchantRepository.Add(new Merchant(createdBy, merchantUserName, merchantPassword, merchantKey, merchantReference));
                work.Commit();
            }

            return CheckExists(merchantKey);
        }

        public bool CheckExists(string merchantKey)
        {
            if (string.IsNullOrWhiteSpace(merchantKey))
                throw new ArgumentNullException(nameof(merchantKey));
            return _merchantRepository.FindAll().FirstOrDefault(m => m.MerchantKey == merchantKey) != null;
        }

        public IQueryable<Merchant> GetAllMerchants()
        {
            var merchants = _merchantRepository.FindAll();
            return merchants.Where(m => m.DateDeleted == null).OrderBy(m => m.MerchantUserName);
        }

        public bool RemoveMerchant(User deletedBy, string merchantKey)
        {
            Merchant merchant = GetAllMerchants().FirstOrDefault(m => m.MerchantKey == merchantKey);
            if (merchant != null)
            {
                using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
                {
                    merchant.Delete(deletedBy);
                    _merchantRepository.Add(merchant);
                    work.Commit();
                }
            }
            // check that it has been removed, and return the inverse result
            return !CheckExists(merchantKey);
        }

        public Merchant GetMerchant(Guid merchantProgrammeId)
        {
            Merchant merchant = _merchantRepository.FindAll().FirstOrDefault(m => m.Programme_id == merchantProgrammeId);
            return merchant;
        }
    }
}
