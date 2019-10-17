using System;
using TechCertain.Services.Interfaces;
using System.Collections.Generic;
using TechCertain.Domain.Entities;
using System.Linq;

namespace TechCertain.Services.Impl
{
	public class CountryService : ICountryService
	{

		public CountryService ()
		{
		}

		#region ICountryService implementation

		public List<Country> GetAllCountries ()
		{
            return new List<Country>();
		}

		public bool AddCountry (Country country)
		{
			return false;
		}

		#endregion
	}
}