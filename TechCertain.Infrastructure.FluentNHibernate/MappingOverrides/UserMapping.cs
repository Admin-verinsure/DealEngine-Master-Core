using System;
using TechCertain.Domain.Entities;
using FluentNHibernate.Mapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace TechCertain.Infrastructure.FluentNHibernate.MappingOverrides
{
    //public class UserMapping : SubclassMap<Account>
    //{
    //    public UserMapping ()
    //    {
    //        Abstract();

    //        Map (x => x.FirstName).Length (20);
    //        Map (x => x.LastName).Length (20);
    //        Map (x => x.Email).Length (60);

    //        Table ("Account");
    //    }
    //}

    public class UserMappingOverride : IAutoMappingOverride<User>
    {
        public void Override(AutoMapping<User> mapping)
        {
            mapping.Id (p => p.Id).GeneratedBy.Assigned();
            mapping.IgnoreProperty (p => p.Location);
            //mapping.IgnoreProperty (p => p.Organisations);
            mapping.IgnoreProperty (p => p.Branches);
			mapping.IgnoreProperty (p => p.Departments);
			mapping.HasManyToMany (p => p.Organisations);
			mapping.HasManyToMany (p => p.Groups);
        }
    }
}