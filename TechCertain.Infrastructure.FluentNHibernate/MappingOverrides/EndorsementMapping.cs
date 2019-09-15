using System;
using TechCertain.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace TechCertain.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class EndorsementMappingOverride : IAutoMappingOverride<Endorsement>
    {
        public void Override(AutoMapping<Endorsement> mapping)
        {
            mapping.Map(x => x.Value).Length(4000);
        }
    }
}
