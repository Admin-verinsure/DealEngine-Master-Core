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
    public class MerchantService : IMerchantService
    {
        IMapperSession<Merchant> _merchantRepository;

        public MerchantService(IMapperSession<Merchant> merchantRepository)
        {
            _merchantRepository = merchantRepository;
        }

        public async Task AddNewMerchant(User createdBy, string merchantUserName, string merchantPassword, string merchantKey, string merchantReference)
        {
            if (string.IsNullOrWhiteSpace(merchantUserName))
                throw new ArgumentNullException(nameof(merchantUserName));
            if (string.IsNullOrWhiteSpace(merchantKey))
                throw new ArgumentNullException(nameof(merchantKey));

            var hasMerchantKey = await CheckExists(merchantKey);
            if (!hasMerchantKey)
                await _merchantRepository.AddAsync(new Merchant(createdBy, merchantUserName, merchantPassword, merchantKey, merchantReference));            
        }

        public async Task<bool> CheckExists(string merchantKey)
        {
            if (string.IsNullOrWhiteSpace(merchantKey))
                throw new ArgumentNullException(nameof(merchantKey));
            return await _merchantRepository.FindAll().FirstOrDefaultAsync(m => m.MerchantKey == merchantKey) != null;
        }

        public async Task<List<Merchant>> GetAllMerchants()
        {
            return await _merchantRepository.FindAll().Where(m => m.DateDeleted == null).OrderBy(m => m.MerchantUserName).ToListAsync();
        }

        public async Task RemoveMerchant(User deletedBy, string merchantKey)
        {
            var merchantList = await GetAllMerchants();
            Merchant merchant = merchantList.FirstOrDefault(m => m.MerchantKey == merchantKey);
            if (merchant != null)
            {
                merchant.Delete(deletedBy);
                await _merchantRepository.UpdateAsync(merchant);

            }
            // check that it has been removed, and return the inverse result
            if (await CheckExists(merchantKey))
                throw new Exception("Should be removed");
        }

        public async Task<Merchant> GetMerchant(Guid merchantProgrammeId)
        {
            return await _merchantRepository.FindAll().FirstOrDefaultAsync(m => m.Programme_id == merchantProgrammeId);
        }
    }
}
