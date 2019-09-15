using FluentNHibernate.Automapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Infrastructure.FluentNHibernate.MappingConventions
{
    public class DefaultMappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.Namespace == typeof(Organisation).Namespace
                && !type.IsDefined(typeof(CompilerGeneratedAttribute), false) ; // see http://stackoverflow.com/a/11447966/84590
        }
        
        public override bool IsComponent(Type type)
        {
            return typeof(ValueObject).IsAssignableFrom(type) || typeof(ValueObject<>).IsAssignableFrom(type);
        }
        
    }
}
