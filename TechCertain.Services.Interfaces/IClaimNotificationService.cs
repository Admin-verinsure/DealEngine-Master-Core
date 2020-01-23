using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClaimNotificationService
    {
        Task<ClaimNotification> GetClaimNotificationById(Guid claimId);
    }
}
