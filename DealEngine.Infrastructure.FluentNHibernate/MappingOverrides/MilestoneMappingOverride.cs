using DealEngine.Domain.Entities;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{

    public class MilestoneMappingOverride : IAutoMappingOverride<Milestone>
    {
        public void Override(AutoMapping<Milestone> mapping)
        {
        }
    }

    public class AdvisoryMappingOverride : IAutoMappingOverride<Advisory>
    {
        public void Override(AutoMapping<Advisory> mapping)
        {
            mapping.Not.LazyLoad();
        }
    }
}