using System;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using TechCertain.Domain.Entities;

namespace TechCertain.Infrastructure.FluentNHibernate
{
	public class ApplicationGroupMapping : IAutoMappingOverride<ApplicationGroup>
	{
		public void Override (AutoMapping<ApplicationGroup> mapping)
		{
			mapping.HasManyToMany (g => g.Users);
			mapping.HasManyToMany (g => g.Roles);
		}
	}
}

