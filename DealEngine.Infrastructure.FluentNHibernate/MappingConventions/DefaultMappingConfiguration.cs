using FluentNHibernate.Automapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Infrastructure.FluentNHibernate.MappingConventions
{
    public class DefaultMappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.Namespace == typeof(Organisation).Namespace
                && !type.IsDefined(typeof(CompilerGeneratedAttribute), false) ; // see http://stackoverflow.com/a/11447966/84590
        }       
        
    }
}
