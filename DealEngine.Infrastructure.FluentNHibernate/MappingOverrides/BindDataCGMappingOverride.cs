using System;
using DealEngine.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{
    //BindDataCG
    public class BindDataCGMappingOverride : IAutoMappingOverride<BindDataCG>
    {
        public void Override(AutoMapping<BindDataCG> mapping)
        {
            //mapping.Not.LazyLoad();
           mapping.References(l => l.Location).Not.LazyLoad();
        }
    }
}
