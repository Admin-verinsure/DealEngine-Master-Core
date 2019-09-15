using System;
using TechCertain.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace TechCertain.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class EmailTemplateMappingOverride : IAutoMappingOverride<EmailTemplate>
    {
        public void Override(AutoMapping<EmailTemplate> mapping)
        {
            mapping.Map(x => x.Body).Length(4000);
        }
    }
}
