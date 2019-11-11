using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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

        public ChangeReason CreateChangeReason(User createdBy, ChangeReason changeReason)
        {
            ChangeReason change = new ChangeReason(createdBy);
            change.DealId = changeReason.DealId;
            change.ChangeType = changeReason.ChangeType;
            change.Reason = changeReason.Reason;
            change.ReasonDesc = changeReason.ReasonDesc;
            //change.EffectiveDate = changeReason.EffectiveDate;
            _changeReason.AddAsync(change);
            return change;
        }
    }
}
