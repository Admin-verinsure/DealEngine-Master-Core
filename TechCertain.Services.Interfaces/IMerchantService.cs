using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IMerchantService
    {

        bool AddNewMerchant(User createdBy, string merchantUserName, string merchantPassword, string merchantKey, string merchantReference);

        bool RemoveMerchant(User deletedBy, string merchantKey);

        bool CheckExists(string merchantKey);

        IQueryable<Merchant> GetAllMerchants();

        Merchant GetMerchant(Guid ProgrammeId);

    }
}

