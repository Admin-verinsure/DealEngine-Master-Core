using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
	public class Country : EntityBase
	{
		protected Country () : base (null) { }

        public Country(User createdBy, string name)
			: base (createdBy)
        {
            Name = name;
        }
        
		public virtual string Name { get; set; }

        public virtual string CurrencyCode { get; set; }

        public virtual string CurrencySymbol { get; set; }

        public virtual string CurrencyDesc { get; set; }

		public virtual Region Region { get; set; }
    }

	public class Region : EntityBase
	{
		public virtual string Name { get; set; }

		public virtual IList<Country> Countries { get; set; }

		protected Region () : base (null) { }

		public Region (User createdBy, string name)
			: base (createdBy)
        {
			Name = name;
			Countries = new List<Country> ();
		}

		/// <summary>
		/// Adds the specified Country to this Region. If it belongs to another Region, it is automatically removed from that Region.
		/// </summary>
		/// <param name="country">Country.</param>
		public virtual void AddCountry (Country country)
		{
			if (Countries.Contains (country))
				return;

			Countries.Add (country);

			if (country.Region != null)
				country.Region.Countries.Remove (country);

			country.Region = this;
		}
	}
}

