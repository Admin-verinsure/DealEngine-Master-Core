using DealEngine.Domain.Entities;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class PreRenewOrRefDataMappingOverride : IAutoMappingOverride<PreRenewOrRefData>
    {
        public void Override(AutoMapping<PreRenewOrRefData> mapping)
        {
            mapping.Map(x => x.PIBoundLimit).Length(200000);
            mapping.Map(x => x.PIBoundPremium).Length(200000);
            mapping.Map(x => x.PIRetro).Length(200000);
            mapping.Map(x => x.GLRetro).Length(200000);
            mapping.Map(x => x.DORetro).Length(200000);
            mapping.Map(x => x.ELRetro).Length(200000);
            mapping.Map(x => x.EDRetro).Length(200000);
            mapping.Map(x => x.SLRetro).Length(200000);
            mapping.Map(x => x.CLRetro).Length(200000);
            mapping.Map(x => x.EndorsementTitle).Length(200000);
            mapping.Map(x => x.EndorsementProduct).Length(200000);
            mapping.Map(x => x.EndorsementText).Length(200000);
        }
    }
}
