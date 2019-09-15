using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;
using System;
using TechCertain.Domain.Entities;

namespace TechCertain.Infrastructure.FluentNHibernate.Mapping
{
	//public class ProductMapping : SubclassMap<Product>
	//{
	//    public ProductMapping()
	//    {
	//        Abstract();
	//        Map(x => x.Name);

	//        Table("Product");
	//    }
	//}

	public class ProductMappingOverride : IAutoMappingOverride<Product>
	{
		public void Override (AutoMapping<Product> mapping)
		{
			//mapping.Id (p => p.Id).GeneratedBy.Assigned ();
			//mapping.IgnoreProperty (p => p.Languages);
			mapping.HasMany (p => p.Languages).Element ("Value");
			mapping.HasManyToMany (p => p.Documents);
			mapping.HasMany (p => p.ChildProducts);
			//mapping.HasManyToMany (p => p.DataSharedWith);
 		}
	}
}
