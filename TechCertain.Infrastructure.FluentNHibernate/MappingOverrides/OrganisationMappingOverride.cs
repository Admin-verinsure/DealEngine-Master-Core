using TechCertain.Domain.Entities;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace TechCertain.Infrastructure.FluentNHibernate.MappingOverrides
{
    public class OrganisationMappingOverride : IAutoMappingOverride<Organisation>
	{
		public OrganisationMappingOverride ()
		{
			//Abstract ();
			//Map (x => x.Name);

            //HasMany<Product>(x => x.Products);

            //HasManyToMany<User> (x => x.Users).Table ("CompanyUser").Inverse ();

            //HasMany<Branch> (x => x.Branches).Inverse ().Cascade.All ();
            //HasMany<Department> (x => x.Departments).Inverse ().Cascade.All ();

			//Table ("Organisation");
		}        
        public void Override(AutoMapping<Organisation> mapping)
		{
			mapping.Id(p => p.Id).GeneratedBy.Assigned();
            //mapping.HasMany<Product_Old>(x => x.Products);
        }
    }
}