using System;
using DealEngine.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingOverrides
{

    public class UserMappingOverride : IAutoMappingOverride<User>
    {
        public void Override(AutoMapping<User> mapping)
        {
            mapping.Id (p => p.Id).GeneratedBy.Assigned();
            mapping.IgnoreProperty (p => p.Location);
            mapping.IgnoreProperty (p => p.Branches);
			mapping.IgnoreProperty (p => p.Departments);
			mapping.HasManyToMany (p => p.Organisations).Not.LazyLoad();
        }
    }
}