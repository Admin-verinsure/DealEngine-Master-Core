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

    }
}
