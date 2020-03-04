using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class ProductPackage : EntityBase, IAggregateRoot
	{
		public virtual IList<Product> Products { get; protected set; }

		public virtual decimal Discount { get; set; }

		protected ProductPackage () : this (null, new List<Product>()) { }

		public ProductPackage (User createdBy, List<Product> products)
			: base(createdBy)
		{
			Products = products;;
		}

		public virtual bool Contains (Product product)
		{
			return Products.Contains (product);
		}

		public virtual void SetDiscount (decimal discount)
		{
			Discount = Math.Min (100m, Math.Max (0m, discount));
		}

		public virtual bool DoesProgrammeMatchPackage (ClientProgramme clientProgramme)
		{
			bool matches = true;
			IList<Product> programmeProducts = clientProgramme.GetSelectedProducts () as IList<Product>;

			foreach (Product product in Products)
				if (!programmeProducts.Contains(product)) {
					matches = false;
					break;
				}
			return matches;
		}
	}
}

