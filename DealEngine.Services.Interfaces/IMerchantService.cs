using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IMerchantService
    {

        Task AddNewMerchant(User createdBy, string merchantUserName, string merchantPassword, string merchantKey, string merchantReference);

        Task RemoveMerchant(User deletedBy, string merchantKey);

        Task<bool> CheckExists(string merchantKey);

        Task<List<Merchant>> GetAllMerchants();

        Task<Merchant> GetMerchant(Guid ProgrammeId);

    }
}

