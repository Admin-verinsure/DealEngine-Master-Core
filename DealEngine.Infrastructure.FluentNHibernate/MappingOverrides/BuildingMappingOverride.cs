using System;
using DealEngine.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{
    //BindDataCG
    public class BuildingMappingOverride : IAutoMappingOverride<Building>
    {
        public void Override(AutoMapping<Building>mapping)
        {
            //mapping.Not.LazyLoad();
            mapping.References(l => l.Location).Not.LazyLoad();
        }
    }
}
