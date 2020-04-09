using System;
using DealEngine.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class ProgrammeMappingOverride : IAutoMappingOverride<Programme>
    {
        public void Override(AutoMapping<Programme> mapping)
        {
            mapping.Map(x => x.Declaration).Length(4000);
            mapping.Map(x => x.StopAgreementMessage).Length(4000);
            mapping.Map(x => x.SubsystemMessage).Length(4000);
            mapping.Map(x => x.NoPaymentRequiredMessage).Length(4000);
        }
    }
}
