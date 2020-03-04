using DealEngine.Domain.Entities;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class EGlobalResponseMappingOverride : IAutoMappingOverride<EGlobalResponse>
    {
        public void Override(AutoMapping<EGlobalResponse> mapping)
        {
            mapping.Map(x => x.ResponseXML).Length(100000);
        }
    }
}
