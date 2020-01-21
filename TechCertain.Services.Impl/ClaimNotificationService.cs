using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;

namespace TechCertain.Services.Impl
{
    public class ClaimNotificationService : IClaimNotificationService
    {
        IMapperSession<ClaimNotification> _claimNotificationRepository;

        public ClaimNotificationService(IMapperSession<ClaimNotification> claimNotificationRepository)
        {
            _claimNotificationRepository = claimNotificationRepository;
        }

        public async Task<ClaimNotification> GetClaimNotificationById(Guid claimId)
        {
            return await _claimNotificationRepository.GetByIdAsync(claimId);
        }
    }
}

