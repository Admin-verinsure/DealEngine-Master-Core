using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class MerchantService : IMerchantService
    {
        IMapperSession<Merchant> _merchantRepository;

        public MerchantService(IMapperSession<Merchant> merchantRepository)
        {
            _merchantRepository = merchantRepository;
        }

        public bool AddNewMerchant(User createdBy, string merchantUserName, string merchantPassword, string merchantKey, string merchantReference)
        {
            if (string.IsNullOrWhiteSpace(merchantUserName))
                throw new ArgumentNullException(nameof(merchantUserName));
            if (string.IsNullOrWhiteSpace(merchantKey))
                throw new ArgumentNullException(nameof(merchantKey));

            _merchantRepository.AddAsync(new Merchant(createdBy, merchantUserName, merchantPassword, merchantKey, merchantReference));
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
                merchant.Delete(deletedBy);
                _merchantRepository.RemoveAsync(merchant);

            }
            // check that it has been removed, and return the inverse result
            return !CheckExists(merchantKey);
        }

        public Merchant GetMerchant(Guid merchantProgrammeId)
        {
            return _merchantRepository.FindAll().FirstOrDefault(m => m.Programme_id == merchantProgrammeId);
        }
    }
}
