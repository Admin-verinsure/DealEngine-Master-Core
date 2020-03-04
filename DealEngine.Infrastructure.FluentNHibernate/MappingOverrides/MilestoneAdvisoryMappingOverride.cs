using System;
using DealEngine.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class MilestoneAdvisoryMappingOverride : IAutoMappingOverride<Advisory>
    {
        public void Override(AutoMapping<Advisory> mapping)
        {
            mapping.Map(x => x.Description).Length(10000);
        }
    }
}
