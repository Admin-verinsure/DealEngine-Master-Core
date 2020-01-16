using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
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
