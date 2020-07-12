using System;
using DealEngine.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{

    public class ClientInformationSheetMappingOverride : IAutoMappingOverride<ClientInformationSheet>
    {
        public void Override(AutoMapping<ClientInformationSheet> mapping)
        {
            mapping.References(p => p.Programme).Not.LazyLoad();
            mapping.References(x => x.RevenueData).Not.LazyLoad();
        }
    }

    public class SubClientInformationSheetMappingOverride : IAutoMappingOverride<SubClientInformationSheet>
    {
        public void Override(AutoMapping<SubClientInformationSheet> mapping)
        {
            mapping.References(p => p.Programme).Not.LazyLoad();
        }
    }

}