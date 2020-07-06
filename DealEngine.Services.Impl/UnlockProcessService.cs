using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
    public class UnlockProcessService : IUnlockProcessService
    {
        IMapperSession<UnlockReason> _changeReason;

        public UnlockProcessService(IMapperSession<UnlockReason> changeReason)
        {
            _changeReason = changeReason;
        }

        public async Task CreateUnlockReason(User createdBy, UnlockReason changeReason)
        {
            UnlockReason change = new UnlockReason(createdBy);
            change.DealId = changeReason.DealId;
            change.Reason = changeReason.Reason;
            //change.EffectiveDate = changeReason.EffectiveDate;
            await _changeReason.AddAsync(change);            
        }
    }
}
