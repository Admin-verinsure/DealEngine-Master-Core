using System;
using DealEngine.Services.Interfaces;
using System.Collections.Generic;
using DealEngine.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace DealEngine.Services.Impl
{
	public class CountryService : ICountryService
	{

		public CountryService ()
		{
		}

		#region ICountryService implementation

		public async Task<List<Country>> GetAllCountries ()
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