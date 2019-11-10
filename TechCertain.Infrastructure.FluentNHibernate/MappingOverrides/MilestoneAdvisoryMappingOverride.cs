using System;
using TechCertain.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace TechCertain.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class MilestoneAdvisoryMappingOverride : IAutoMappingOverride<Advisory>
    {
        public void Override(AutoMapping<Advisory> mapping)
        {
            mapping.Map(x => x.Description).Length(4000);
        }
    }
}
