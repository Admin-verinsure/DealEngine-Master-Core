using System;
using DealEngine.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class SystemEmailMappingOverride : IAutoMappingOverride<SystemEmail>
    {
        public void Override(AutoMapping<SystemEmail> mapping)
        {
            mapping.Map(x => x.Body).Length(4000);
        }
    }
}
