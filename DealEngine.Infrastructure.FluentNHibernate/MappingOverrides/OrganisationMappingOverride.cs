using System;
using DealEngine.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{

    public class OrganisationMappingOverride : IAutoMappingOverride<Organisation>
    {
        public void Override(AutoMapping<Organisation> mapping)
        {
            mapping.Id(p => p.Id).GeneratedBy.Assigned();
        }
    }

}