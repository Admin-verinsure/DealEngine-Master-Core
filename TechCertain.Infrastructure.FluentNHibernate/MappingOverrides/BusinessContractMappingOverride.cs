using TechCertain.Domain.Entities;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace TechCertain.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class BusinessContractMappingOverride : IAutoMappingOverride<BusinessContract>
    {
        public void Override(AutoMapping<BusinessContract> mapping)
        {
            mapping.Map(x => x.ContractTitle).Length(100000);
        }
    }
}
