using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class ProductGrouping : EntityBase, IAggregateRoot
	{
		public virtual IList<Product> Products { get; protected set; }

		protected ProductGrouping ()
			: this (null)
		{ }

		public ProductGrouping (User createdBy)
			: base(createdBy)
		{
			Products = new List<Product> ();
		}

		public virtual void Add (Product product)
		{
			if (Products.Contains (product))
				return;
			Products.Add (product);
		}

		public virtual bool Contains (Product product)
		{
			return Products.Contains (product);
		}
	}
}

