using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
    public class ChangeProcessService : IChangeProcessService
    {
        IMapperSession<ChangeReason> _changeReason;

        public ChangeProcessService(IMapperSession<ChangeReason> changeReason)
        {
            _changeReason = changeReason;
        }

        public async Task CreateChangeReason(User createdBy, ChangeReason changeReason)
        {
            ChangeReason change = new ChangeReason(createdBy);
            change.DealId = changeReason.DealId;
            change.ChangeType = changeReason.ChangeType;
            change.Reason = changeReason.Reason;
            change.ReasonDesc = changeReason.ReasonDesc;
            //change.EffectiveDate = changeReason.EffectiveDate;
            await _changeReason.AddAsync(change);
        }
    }
}
