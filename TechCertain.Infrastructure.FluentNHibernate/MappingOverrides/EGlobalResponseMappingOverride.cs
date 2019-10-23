using TechCertain.Domain.Entities;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace TechCertain.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class EGlobalResponseMappingOverride : IAutoMappingOverride<EGlobalResponse>
    {
        public void Override(AutoMapping<EGlobalResponse> mapping)
        {
            mapping.Map(x => x.ResponseXML).Length(100000);
        }
    }
}
